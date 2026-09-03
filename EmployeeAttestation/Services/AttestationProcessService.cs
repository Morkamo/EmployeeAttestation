using System.Globalization;
using EmployeeAttestation.Data;
using EmployeeAttestation.Extra;
using EmployeeAttestation.Models;
using Microsoft.Data.Sqlite;

namespace EmployeeAttestation.Services;

public sealed class AttestationProcessService
{
    private const string DateTimeFormat = "yyyy-MM-dd'T'HH:mm:ss.fffK";
    private readonly DatabaseManager databaseManager;

    public AttestationProcessService(DatabaseManager databaseManager)
    {
        this.databaseManager = databaseManager ?? throw new ArgumentNullException(nameof(databaseManager));
    }

    public List<AttestationCommissionMember> GetMembers(int attestationId)
    {
        ValidateId(attestationId, nameof(attestationId));
        try
        {
            using SqliteConnection connection = databaseManager.CreateConnection();
            connection.Open();
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                """
                SELECT id, attestation_id, commission_member_id, member_full_name,
                       role, sort_order, is_present
                FROM attestation_commission_members
                WHERE attestation_id = $attestationId
                ORDER BY sort_order, member_full_name;
                """;
            command.Parameters.AddWithValue("$attestationId", attestationId);
            using SqliteDataReader reader = command.ExecuteReader();
            List<AttestationCommissionMember> result = [];
            while (reader.Read()) result.Add(ReadMember(reader));
            return result;
        }
        catch (Exception exception) when (IsDatabaseException(exception))
        {
            throw new AttestationProcessServiceException("Не удалось загрузить состав аттестационной комиссии.", exception);
        }
    }

    public List<AttestationCriterion> GetCriteria(int attestationId)
    {
        ValidateId(attestationId, nameof(attestationId));
        try
        {
            using SqliteConnection connection = databaseManager.CreateConnection();
            connection.Open();
            return ReadCriteria(connection, null, attestationId);
        }
        catch (Exception exception) when (IsDatabaseException(exception))
        {
            throw new AttestationProcessServiceException("Не удалось загрузить критерии аттестации.", exception);
        }
    }

    public List<AttestationScore> GetScores(int attestationId)
    {
        ValidateId(attestationId, nameof(attestationId));
        try
        {
            using SqliteConnection connection = databaseManager.CreateConnection();
            connection.Open();
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                """
                SELECT id, attestation_id, attestation_commission_member_id,
                       attestation_criterion_id, score
                FROM attestation_scores
                WHERE attestation_id = $attestationId
                  AND attestation_commission_member_id IS NOT NULL
                  AND attestation_criterion_id IS NOT NULL;
                """;
            command.Parameters.AddWithValue("$attestationId", attestationId);
            using SqliteDataReader reader = command.ExecuteReader();
            List<AttestationScore> result = [];
            while (reader.Read())
            {
                result.Add(new AttestationScore
                {
                    Id = reader.GetInt32(0),
                    AttestationId = reader.GetInt32(1),
                    AttestationCommissionMemberId = reader.GetInt32(2),
                    AttestationCriterionId = reader.GetInt32(3),
                    Score = reader.GetInt32(4)
                });
            }
            return result;
        }
        catch (Exception exception) when (IsDatabaseException(exception))
        {
            throw new AttestationProcessServiceException("Не удалось загрузить оценки.", exception);
        }
    }

    public List<AttestationVote> GetVotes(int attestationId)
    {
        ValidateId(attestationId, nameof(attestationId));
        try
        {
            using SqliteConnection connection = databaseManager.CreateConnection();
            connection.Open();
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                """
                SELECT id, attestation_id, attestation_commission_member_id, vote
                FROM attestation_votes
                WHERE attestation_id = $attestationId
                  AND attestation_commission_member_id IS NOT NULL;
                """;
            command.Parameters.AddWithValue("$attestationId", attestationId);
            using SqliteDataReader reader = command.ExecuteReader();
            List<AttestationVote> result = [];
            while (reader.Read())
            {
                result.Add(new AttestationVote
                {
                    Id = reader.GetInt32(0),
                    AttestationId = reader.GetInt32(1),
                    AttestationCommissionMemberId = reader.GetInt32(2),
                    Vote = reader.GetString(3)
                });
            }
            return result;
        }
        catch (Exception exception) when (IsDatabaseException(exception))
        {
            throw new AttestationProcessServiceException("Не удалось загрузить голосование.", exception);
        }
    }

    public void SaveProgress(
        int attestationId,
        IReadOnlyCollection<int> presentMemberIds,
        IReadOnlyCollection<AttestationScore> scores)
    {
        ValidateId(attestationId, nameof(attestationId));
        ArgumentNullException.ThrowIfNull(presentMemberIds);
        ArgumentNullException.ThrowIfNull(scores);
        ExecuteTransaction("Не удалось сохранить оценки.", (connection, transaction) =>
        {
            EnsureStatus(connection, transaction, attestationId, AttestationStatusHelper.InProgress);
            SaveProgressCore(connection, transaction, attestationId, presentMemberIds, scores);
        });
    }

    public void TransitionToDecision(
        int attestationId,
        IReadOnlyCollection<int> presentMemberIds,
        IReadOnlyCollection<AttestationScore> scores)
    {
        ValidateId(attestationId, nameof(attestationId));
        ArgumentNullException.ThrowIfNull(presentMemberIds);
        ArgumentNullException.ThrowIfNull(scores);
        ExecuteTransaction("Не удалось перейти к решению комиссии.", (connection, transaction) =>
        {
            EnsureStatus(connection, transaction, attestationId, AttestationStatusHelper.InProgress);
            SaveProgressCore(connection, transaction, attestationId, presentMemberIds, scores);
            int presentCount = CountPresentMembers(connection, transaction, attestationId);
            if (presentCount == 0)
            {
                throw new AttestationProcessServiceException("Отметьте хотя бы одного присутствующего члена комиссии.");
            }
            int criteriaCount = CountCriteria(connection, transaction, attestationId);
            int scoreCount = CountScoresForPresentMembers(connection, transaction, attestationId);
            if (criteriaCount == 0 || scoreCount != presentCount * criteriaCount)
            {
                throw new AttestationProcessServiceException(
                    "Выставьте оценки по каждому критерию всем присутствующим членам комиссии.");
            }

            (double? professional, double? personal, double? managerial, double? overall) =
                CalculateAverages(connection, transaction, attestationId);
            using SqliteCommand command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText =
                """
                UPDATE attestations
                SET professional_average = $professional,
                    personal_average = $personal,
                    managerial_average = $managerial,
                    overall_average = $overall,
                    present_members_count = $presentCount,
                    status = $decision
                WHERE id = $attestationId AND status = $inProgress;
                """;
            command.Parameters.AddWithValue("$professional", DbValue(professional));
            command.Parameters.AddWithValue("$personal", DbValue(personal));
            command.Parameters.AddWithValue("$managerial", DbValue(managerial));
            command.Parameters.AddWithValue("$overall", DbValue(overall));
            command.Parameters.AddWithValue("$presentCount", presentCount);
            command.Parameters.AddWithValue("$decision", AttestationStatusHelper.Decision);
            command.Parameters.AddWithValue("$attestationId", attestationId);
            command.Parameters.AddWithValue("$inProgress", AttestationStatusHelper.InProgress);
            if (command.ExecuteNonQuery() == 0) ThrowStatusChanged();
        });
    }

    public void SaveDecision(
        int attestationId,
        string? decision,
        string? recommendations,
        IReadOnlyCollection<AttestationVote> votes)
    {
        ValidateId(attestationId, nameof(attestationId));
        ArgumentNullException.ThrowIfNull(votes);
        ExecuteTransaction("Не удалось сохранить решение комиссии.", (connection, transaction) =>
        {
            EnsureStatus(connection, transaction, attestationId, AttestationStatusHelper.Decision);
            SaveDecisionCore(connection, transaction, attestationId, decision, recommendations, votes);
        });
    }

    public void Complete(
        int attestationId,
        string? decision,
        string? recommendations,
        IReadOnlyCollection<AttestationVote> votes)
    {
        ValidateId(attestationId, nameof(attestationId));
        ArgumentNullException.ThrowIfNull(votes);
        string normalizedDecision = decision?.Trim() ?? string.Empty;
        if (normalizedDecision.Length == 0)
        {
            throw new AttestationProcessServiceException("Введите решение комиссии.");
        }

        ExecuteTransaction("Не удалось завершить аттестацию.", (connection, transaction) =>
        {
            EnsureStatus(connection, transaction, attestationId, AttestationStatusHelper.Decision);
            SaveDecisionCore(connection, transaction, attestationId, normalizedDecision, recommendations, votes);
            int presentCount = CountPresentMembers(connection, transaction, attestationId);
            int voteCount = CountVotes(connection, transaction, attestationId);
            if (presentCount == 0 || voteCount != presentCount)
            {
                throw new AttestationProcessServiceException(
                    "Каждый присутствующий член комиссии должен проголосовать.");
            }
            int criteriaCount = CountCriteria(connection, transaction, attestationId);
            if (CountScoresForPresentMembers(connection, transaction, attestationId) != presentCount * criteriaCount)
            {
                throw new AttestationProcessServiceException("Оценки заполнены не полностью.");
            }

            (int votesFor, int votesAgainst, int votesAbstained) = CountVotesByValue(
                connection,
                transaction,
                attestationId);
            using SqliteCommand command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText =
                """
                UPDATE attestations
                SET decision = $decision,
                    recommendations = $recommendations,
                    votes_for = $votesFor,
                    votes_against = $votesAgainst,
                    votes_abstained = $votesAbstained,
                    completed_at = $completedAt,
                    status = $completed
                WHERE id = $attestationId
                  AND status = $decisionStatus
                  AND overall_average IS NOT NULL;
                """;
            command.Parameters.AddWithValue("$decision", normalizedDecision);
            command.Parameters.AddWithValue("$recommendations", DbValue(NormalizeOptional(recommendations)));
            command.Parameters.AddWithValue("$votesFor", votesFor);
            command.Parameters.AddWithValue("$votesAgainst", votesAgainst);
            command.Parameters.AddWithValue("$votesAbstained", votesAbstained);
            command.Parameters.AddWithValue("$completedAt", DateTime.Now.ToString(DateTimeFormat, CultureInfo.InvariantCulture));
            command.Parameters.AddWithValue("$completed", AttestationStatusHelper.Completed);
            command.Parameters.AddWithValue("$attestationId", attestationId);
            command.Parameters.AddWithValue("$decisionStatus", AttestationStatusHelper.Decision);
            if (command.ExecuteNonQuery() == 0)
            {
                throw new AttestationProcessServiceException(
                    "Итоговые показатели не рассчитаны или статус аттестации изменился.");
            }
        });
    }

    private static void SaveProgressCore(
        SqliteConnection connection,
        SqliteTransaction transaction,
        int attestationId,
        IReadOnlyCollection<int> presentMemberIds,
        IReadOnlyCollection<AttestationScore> scores)
    {
        HashSet<int> presentIds = presentMemberIds.Where(id => id > 0).ToHashSet();
        List<AttestationCommissionMember> members = ReadMembers(connection, transaction, attestationId);
        List<AttestationCriterion> criteria = ReadCriteria(connection, transaction, attestationId);
        HashSet<int> memberIds = members.Select(item => item.Id).ToHashSet();
        if (!presentIds.IsSubsetOf(memberIds))
        {
            throw new AttestationProcessServiceException("Состав комиссии изменился. Обновите форму.");
        }
        Dictionary<int, AttestationCriterion> criteriaById = criteria.ToDictionary(item => item.Id);
        foreach (AttestationScore score in scores)
        {
            if (!presentIds.Contains(score.AttestationCommissionMemberId)
                || !criteriaById.TryGetValue(score.AttestationCriterionId, out AttestationCriterion? criterion))
            {
                throw new AttestationProcessServiceException("Одна из оценок относится к недоступному участнику или критерию.");
            }
            if (score.Score < criterion.MinimumScore || score.Score > criterion.MaximumScore)
            {
                throw new AttestationProcessServiceException(
                    $"Оценка по критерию «{criterion.CriterionName}» должна быть от {criterion.MinimumScore} до {criterion.MaximumScore}.");
            }
        }

        using (SqliteCommand resetPresenceCommand = connection.CreateCommand())
        {
            resetPresenceCommand.Transaction = transaction;
            resetPresenceCommand.CommandText =
                "UPDATE attestation_commission_members SET is_present = 0 WHERE attestation_id = $attestationId;";
            resetPresenceCommand.Parameters.AddWithValue("$attestationId", attestationId);
            resetPresenceCommand.ExecuteNonQuery();
        }
        foreach (int memberId in presentIds)
        {
            using SqliteCommand presenceCommand = connection.CreateCommand();
            presenceCommand.Transaction = transaction;
            presenceCommand.CommandText =
                """
                UPDATE attestation_commission_members
                SET is_present = 1
                WHERE id = $memberId AND attestation_id = $attestationId;
                """;
            presenceCommand.Parameters.AddWithValue("$memberId", memberId);
            presenceCommand.Parameters.AddWithValue("$attestationId", attestationId);
            presenceCommand.ExecuteNonQuery();
        }

        using (SqliteCommand deleteScoresCommand = connection.CreateCommand())
        {
            deleteScoresCommand.Transaction = transaction;
            deleteScoresCommand.CommandText = "DELETE FROM attestation_scores WHERE attestation_id = $attestationId;";
            deleteScoresCommand.Parameters.AddWithValue("$attestationId", attestationId);
            deleteScoresCommand.ExecuteNonQuery();
        }

        Dictionary<int, int> sourceMemberIds = members.ToDictionary(
            item => item.Id,
            item => item.CommissionMemberId
                ?? throw new AttestationProcessServiceException("Исторический участник не поддерживает редактирование оценки."));
        Dictionary<int, int> sourceCriterionIds = criteria.ToDictionary(
            item => item.Id,
            item => item.CriterionId
                ?? throw new AttestationProcessServiceException("Исторический критерий не поддерживает редактирование оценки."));
        foreach (AttestationScore score in scores)
        {
            using SqliteCommand insertCommand = connection.CreateCommand();
            insertCommand.Transaction = transaction;
            insertCommand.CommandText =
                """
                INSERT INTO attestation_scores
                    (attestation_id, commission_member_id, criterion_id, score,
                     attestation_criterion_id, attestation_commission_member_id)
                VALUES
                    ($attestationId, $commissionMemberId, $criterionId, $score,
                     $attestationCriterionId, $attestationCommissionMemberId);
                """;
            insertCommand.Parameters.AddWithValue("$attestationId", attestationId);
            insertCommand.Parameters.AddWithValue("$commissionMemberId", sourceMemberIds[score.AttestationCommissionMemberId]);
            insertCommand.Parameters.AddWithValue("$criterionId", sourceCriterionIds[score.AttestationCriterionId]);
            insertCommand.Parameters.AddWithValue("$score", score.Score);
            insertCommand.Parameters.AddWithValue("$attestationCriterionId", score.AttestationCriterionId);
            insertCommand.Parameters.AddWithValue("$attestationCommissionMemberId", score.AttestationCommissionMemberId);
            insertCommand.ExecuteNonQuery();
        }
    }

    private static void SaveDecisionCore(
        SqliteConnection connection,
        SqliteTransaction transaction,
        int attestationId,
        string? decision,
        string? recommendations,
        IReadOnlyCollection<AttestationVote> votes)
    {
        List<AttestationCommissionMember> members = ReadMembers(connection, transaction, attestationId);
        Dictionary<int, AttestationCommissionMember> presentMembers = members
            .Where(item => item.IsPresent)
            .ToDictionary(item => item.Id);
        foreach (AttestationVote vote in votes)
        {
            if (!presentMembers.ContainsKey(vote.AttestationCommissionMemberId)
                || !AttestationVoteHelper.IsValid(vote.Vote))
            {
                throw new AttestationProcessServiceException("Голосование содержит недопустимое значение.");
            }
        }
        if (votes.Select(item => item.AttestationCommissionMemberId).Distinct().Count() != votes.Count)
        {
            throw new AttestationProcessServiceException("Для одного участника указано несколько голосов.");
        }

        using (SqliteCommand updateCommand = connection.CreateCommand())
        {
            updateCommand.Transaction = transaction;
            updateCommand.CommandText =
                """
                UPDATE attestations
                SET decision = $decision,
                    recommendations = $recommendations
                WHERE id = $attestationId AND status = $status;
                """;
            updateCommand.Parameters.AddWithValue("$decision", DbValue(NormalizeOptional(decision)));
            updateCommand.Parameters.AddWithValue("$recommendations", DbValue(NormalizeOptional(recommendations)));
            updateCommand.Parameters.AddWithValue("$attestationId", attestationId);
            updateCommand.Parameters.AddWithValue("$status", AttestationStatusHelper.Decision);
            if (updateCommand.ExecuteNonQuery() == 0) ThrowStatusChanged();
        }
        using (SqliteCommand deleteCommand = connection.CreateCommand())
        {
            deleteCommand.Transaction = transaction;
            deleteCommand.CommandText = "DELETE FROM attestation_votes WHERE attestation_id = $attestationId;";
            deleteCommand.Parameters.AddWithValue("$attestationId", attestationId);
            deleteCommand.ExecuteNonQuery();
        }
        foreach (AttestationVote vote in votes)
        {
            AttestationCommissionMember member = presentMembers[vote.AttestationCommissionMemberId];
            int sourceMemberId = member.CommissionMemberId
                ?? throw new AttestationProcessServiceException("Исторический участник не поддерживает редактирование голоса.");
            using SqliteCommand insertCommand = connection.CreateCommand();
            insertCommand.Transaction = transaction;
            insertCommand.CommandText =
                """
                INSERT INTO attestation_votes
                    (attestation_id, commission_member_id, vote, attestation_commission_member_id)
                VALUES
                    ($attestationId, $commissionMemberId, $vote, $snapshotMemberId);
                """;
            insertCommand.Parameters.AddWithValue("$attestationId", attestationId);
            insertCommand.Parameters.AddWithValue("$commissionMemberId", sourceMemberId);
            insertCommand.Parameters.AddWithValue("$vote", vote.Vote);
            insertCommand.Parameters.AddWithValue("$snapshotMemberId", vote.AttestationCommissionMemberId);
            insertCommand.ExecuteNonQuery();
        }
    }

    private static (double? Professional, double? Personal, double? Managerial, double? Overall) CalculateAverages(
        SqliteConnection connection,
        SqliteTransaction transaction,
        int attestationId)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText =
            """
            SELECT AVG(CASE WHEN ac.category = 'Professional' THEN s.score END),
                   AVG(CASE WHEN ac.category = 'Personal' THEN s.score END),
                   AVG(CASE WHEN ac.category = 'Managerial' THEN s.score END),
                   AVG(s.score)
            FROM attestation_scores AS s
            INNER JOIN attestation_criteria AS ac ON ac.id = s.attestation_criterion_id
            INNER JOIN attestation_commission_members AS am
                ON am.id = s.attestation_commission_member_id AND am.is_present = 1
            WHERE s.attestation_id = $attestationId;
            """;
        command.Parameters.AddWithValue("$attestationId", attestationId);
        using SqliteDataReader reader = command.ExecuteReader();
        reader.Read();
        return (
            reader.IsDBNull(0) ? null : reader.GetDouble(0),
            reader.IsDBNull(1) ? null : reader.GetDouble(1),
            reader.IsDBNull(2) ? null : reader.GetDouble(2),
            reader.IsDBNull(3) ? null : reader.GetDouble(3));
    }

    private static List<AttestationCommissionMember> ReadMembers(
        SqliteConnection connection,
        SqliteTransaction? transaction,
        int attestationId)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText =
            """
            SELECT id, attestation_id, commission_member_id, member_full_name,
                   role, sort_order, is_present
            FROM attestation_commission_members
            WHERE attestation_id = $attestationId
            ORDER BY sort_order, member_full_name;
            """;
        command.Parameters.AddWithValue("$attestationId", attestationId);
        using SqliteDataReader reader = command.ExecuteReader();
        List<AttestationCommissionMember> result = [];
        while (reader.Read()) result.Add(ReadMember(reader));
        return result;
    }

    private static List<AttestationCriterion> ReadCriteria(
        SqliteConnection connection,
        SqliteTransaction? transaction,
        int attestationId)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText =
            """
            SELECT id, attestation_id, criterion_id, criterion_code, criterion_name,
                   category, minimum_score, maximum_score, managers_only, sort_order
            FROM attestation_criteria
            WHERE attestation_id = $attestationId
            ORDER BY CASE category
                         WHEN 'Professional' THEN 1
                         WHEN 'Personal' THEN 2
                         WHEN 'Managerial' THEN 3
                         ELSE 4
                     END,
                     sort_order,
                     criterion_name;
            """;
        command.Parameters.AddWithValue("$attestationId", attestationId);
        using SqliteDataReader reader = command.ExecuteReader();
        List<AttestationCriterion> result = [];
        while (reader.Read())
        {
            result.Add(new AttestationCriterion
            {
                Id = reader.GetInt32(0),
                AttestationId = reader.GetInt32(1),
                CriterionId = reader.IsDBNull(2) ? null : reader.GetInt32(2),
                CriterionCode = reader.GetString(3),
                CriterionName = reader.GetString(4),
                Category = reader.GetString(5),
                MinimumScore = reader.GetInt32(6),
                MaximumScore = reader.GetInt32(7),
                ManagersOnly = reader.GetInt32(8) == 1,
                SortOrder = reader.GetInt32(9)
            });
        }
        return result;
    }

    private static AttestationCommissionMember ReadMember(SqliteDataReader reader) => new()
    {
        Id = reader.GetInt32(0),
        AttestationId = reader.GetInt32(1),
        CommissionMemberId = reader.IsDBNull(2) ? null : reader.GetInt32(2),
        MemberFullName = reader.GetString(3),
        Role = reader.GetString(4),
        SortOrder = reader.GetInt32(5),
        IsPresent = reader.GetInt32(6) == 1
    };

    private static int CountPresentMembers(SqliteConnection connection, SqliteTransaction transaction, int id) =>
        ExecuteCount(connection, transaction,
            "SELECT COUNT(*) FROM attestation_commission_members WHERE attestation_id = $id AND is_present = 1;", id);

    private static int CountCriteria(SqliteConnection connection, SqliteTransaction transaction, int id) =>
        ExecuteCount(connection, transaction,
            "SELECT COUNT(*) FROM attestation_criteria WHERE attestation_id = $id;", id);

    private static int CountScoresForPresentMembers(SqliteConnection connection, SqliteTransaction transaction, int id) =>
        ExecuteCount(connection, transaction,
            """
            SELECT COUNT(*)
            FROM attestation_scores AS s
            INNER JOIN attestation_commission_members AS m
                ON m.id = s.attestation_commission_member_id AND m.is_present = 1
            WHERE s.attestation_id = $id;
            """, id);

    private static int CountVotes(SqliteConnection connection, SqliteTransaction transaction, int id) =>
        ExecuteCount(connection, transaction,
            "SELECT COUNT(*) FROM attestation_votes WHERE attestation_id = $id;", id);

    private static int ExecuteCount(
        SqliteConnection connection,
        SqliteTransaction transaction,
        string sql,
        int id)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = sql;
        command.Parameters.AddWithValue("$id", id);
        return Convert.ToInt32(command.ExecuteScalar(), CultureInfo.InvariantCulture);
    }

    private static (int For, int Against, int Abstained) CountVotesByValue(
        SqliteConnection connection,
        SqliteTransaction transaction,
        int id)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText =
            """
            SELECT SUM(CASE WHEN vote = 'For' THEN 1 ELSE 0 END),
                   SUM(CASE WHEN vote = 'Against' THEN 1 ELSE 0 END),
                   SUM(CASE WHEN vote = 'Abstained' THEN 1 ELSE 0 END)
            FROM attestation_votes
            WHERE attestation_id = $id;
            """;
        command.Parameters.AddWithValue("$id", id);
        using SqliteDataReader reader = command.ExecuteReader();
        reader.Read();
        return (
            reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
            reader.IsDBNull(1) ? 0 : reader.GetInt32(1),
            reader.IsDBNull(2) ? 0 : reader.GetInt32(2));
    }

    private static void EnsureStatus(
        SqliteConnection connection,
        SqliteTransaction transaction,
        int attestationId,
        string expectedStatus)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = "SELECT status FROM attestations WHERE id = $id LIMIT 1;";
        command.Parameters.AddWithValue("$id", attestationId);
        string? actualStatus = command.ExecuteScalar() as string;
        if (actualStatus is null) throw new AttestationProcessServiceException("Аттестация больше не существует.");
        if (!string.Equals(actualStatus, expectedStatus, StringComparison.Ordinal)) ThrowStatusChanged();
    }

    private void ExecuteTransaction(
        string databaseErrorMessage,
        Action<SqliteConnection, SqliteTransaction> action)
    {
        try
        {
            using SqliteConnection connection = databaseManager.CreateConnection();
            connection.Open();
            using SqliteTransaction transaction = connection.BeginTransaction();
            action(connection, transaction);
            transaction.Commit();
        }
        catch (AttestationProcessServiceException)
        {
            throw;
        }
        catch (Exception exception) when (IsDatabaseException(exception))
        {
            throw new AttestationProcessServiceException(databaseErrorMessage, exception);
        }
    }

    private static object DbValue(object? value) => value ?? DBNull.Value;

    private static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static void ThrowStatusChanged() => throw new AttestationProcessServiceException(
        "Статус аттестации изменился. Обновите данные и повторите попытку.");

    private static void ValidateId(int id, string parameterName)
    {
        if (id <= 0) throw new ArgumentOutOfRangeException(parameterName);
    }

    private static bool IsDatabaseException(Exception exception) =>
        exception is SqliteException or IOException or UnauthorizedAccessException;
}

public sealed class AttestationProcessServiceException : Exception
{
    public AttestationProcessServiceException(string message) : base(message) { }
    public AttestationProcessServiceException(string message, Exception innerException) : base(message, innerException) { }
}
