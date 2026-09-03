using EmployeeAttestation.Extra;
using EmployeeAttestation.Models;
using EmployeeAttestation.Services;
using EmployeeAttestation.Styles;

namespace EmployeeAttestation.Forms.Dialogs;

public partial class AttestationProcessForm : Form
{
    private readonly AttestationService? attestationService;
    private readonly AttestationProcessService? processService;
    private readonly int attestationId;
    private Attestation? attestation;
    private List<AttestationCriterion> criteria = [];
    private List<AttestationCommissionMember> members = [];
    private readonly Dictionary<(int MemberId, int CriterionId), ComboBox> scoreControls = [];
    private bool loading;

    public AttestationProcessForm()
    {
        InitializeComponent();
        AppControlStyles.ApplyPrimaryButton(transitionButton);
        AppControlStyles.ApplyPrimaryButton(completeButton);
        AppControlStyles.ApplySecondaryButton(saveScoresButton);
        AppControlStyles.ApplySecondaryButton(saveDecisionButton);
        AppControlStyles.ApplySecondaryButton(closeButton);
        AppControlStyles.ApplyGrid(votesGrid);
        LoadWindowIcon();
    }

    public AttestationProcessForm(
        AttestationService attestationService,
        AttestationProcessService processService,
        int attestationId)
        : this()
    {
        this.attestationService = attestationService ?? throw new ArgumentNullException(nameof(attestationService));
        this.processService = processService ?? throw new ArgumentNullException(nameof(processService));
        this.attestationId = attestationId > 0 ? attestationId : throw new ArgumentOutOfRangeException(nameof(attestationId));
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        LoadProcess();
    }

    private void PresenceList_ItemCheck(object? sender, ItemCheckEventArgs e)
    {
        if (loading || !IsHandleCreated) return;
        BeginInvoke(UpdateScoreAvailability);
    }

    private void SaveScoresButton_Click(object? sender, EventArgs e) => SaveProgress();
    private void TransitionButton_Click(object? sender, EventArgs e) => TransitionToDecision();
    private void SaveDecisionButton_Click(object? sender, EventArgs e) => SaveDecision();
    private void CompleteButton_Click(object? sender, EventArgs e) => CompleteAttestation();
    private void VotesGrid_CellValueChanged(object? sender, DataGridViewCellEventArgs e) => UpdateVoteCounts();

    private void LoadProcess(bool selectDecisionTab = false)
    {
        if (attestationService is null || processService is null) return;
        loading = true;
        try
        {
            attestation = attestationService.GetById(attestationId);
            if (attestation is null)
            {
                ShowError("Аттестация больше не существует.");
                Close();
                return;
            }
            if (attestation.Status is not (AttestationStatusHelper.InProgress
                or AttestationStatusHelper.Decision
                or AttestationStatusHelper.Completed))
            {
                ShowError("Проведение доступно только для начатой аттестации.");
                Close();
                return;
            }

            criteria = processService.GetCriteria(attestationId);
            members = processService.GetMembers(attestationId);
            List<AttestationScore> scores = processService.GetScores(attestationId);
            List<AttestationVote> votes = processService.GetVotes(attestationId);
            FillBasicData();
            FillPresence();
            BuildScoreMatrix(scores);
            FillDecision(votes);
            ConfigureMode();
            UpdateAverageLabels();
            UpdateVoteCounts();
            if (selectDecisionTab) tabs.SelectedTab = decisionTab;
        }
        catch (Exception exception) when (exception is AttestationServiceException or AttestationProcessServiceException)
        {
            ShowError(exception.Message);
        }
        finally
        {
            loading = false;
        }
    }

    private void FillBasicData()
    {
        if (attestation is null) return;
        employeeValueLabel.Text = attestation.EmployeeFullName;
        departmentValueLabel.Text = attestation.DepartmentName;
        positionValueLabel.Text = attestation.PositionName;
        dateValueLabel.Text = attestation.AttestationDate?.ToString("dd.MM.yyyy") ?? "—";
        commissionValueLabel.Text = attestation.CommissionName;
        statusValueLabel.Text = AttestationStatusHelper.GetDisplayName(attestation.Status);
        managerialValueLabel.Text = attestation.EvaluateManagerial ? "Да" : "Нет";
    }

    private void FillPresence()
    {
        presenceList.Items.Clear();
        for (int index = 0; index < members.Count; index++)
        {
            AttestationCommissionMember member = members[index];
            presenceList.Items.Add($"{member.MemberFullName} — {CommissionRoleHelper.GetDisplayName(member.Role)}");
            presenceList.SetItemChecked(index, member.IsPresent);
        }
    }

    private void BuildScoreMatrix(IReadOnlyCollection<AttestationScore> scores)
    {
        scoreControls.Clear();
        scoresHostPanel.Controls.Clear();
        TableLayoutPanel matrix = new()
        {
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            BackColor = AppColors.Surface,
            ColumnCount = members.Count + 1,
            RowCount = criteria.Count + 1,
            Dock = DockStyle.Top,
            Padding = new Padding(8)
        };
        matrix.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 310F));
        foreach (AttestationCommissionMember _ in members) matrix.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 155F));
        matrix.RowStyles.Add(new RowStyle(SizeType.Absolute, 54F));
        matrix.Controls.Add(CreateMatrixHeader("Критерий"), 0, 0);
        for (int memberIndex = 0; memberIndex < members.Count; memberIndex++)
            matrix.Controls.Add(CreateMatrixHeader(members[memberIndex].MemberFullName), memberIndex + 1, 0);

        Dictionary<(int, int), int> values = scores.ToDictionary(
            item => (item.AttestationCommissionMemberId, item.AttestationCriterionId),
            item => item.Score);
        for (int criterionIndex = 0; criterionIndex < criteria.Count; criterionIndex++)
        {
            AttestationCriterion criterion = criteria[criterionIndex];
            matrix.RowStyles.Add(new RowStyle(SizeType.Absolute, 52F));
            Label criterionLabel = new()
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = AppColors.TextPrimary,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(8, 0, 8, 0),
                Text = $"{criterion.CriterionName} ({criterion.MinimumScore}–{criterion.MaximumScore})"
            };
            matrix.Controls.Add(criterionLabel, 0, criterionIndex + 1);
            for (int memberIndex = 0; memberIndex < members.Count; memberIndex++)
            {
                AttestationCommissionMember member = members[memberIndex];
                ComboBox scoreBox = new()
                {
                    Dock = DockStyle.Fill,
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Font = new Font("Segoe UI", 10F),
                    Margin = new Padding(10, 9, 10, 8)
                };
                scoreBox.Items.Add(string.Empty);
                for (int score = criterion.MinimumScore; score <= criterion.MaximumScore; score++) scoreBox.Items.Add(score);
                if (values.TryGetValue((member.Id, criterion.Id), out int selectedScore)) scoreBox.SelectedItem = selectedScore;
                else scoreBox.SelectedIndex = 0;
                scoreControls[(member.Id, criterion.Id)] = scoreBox;
                matrix.Controls.Add(scoreBox, memberIndex + 1, criterionIndex + 1);
            }
        }
        scoresHostPanel.Controls.Add(matrix);
        UpdateScoreAvailability();
    }

    private static Label CreateMatrixHeader(string text) => new()
    {
        BackColor = AppColors.HeaderBackground,
        Dock = DockStyle.Fill,
        Font = new Font("Segoe UI Semibold", 9.5F),
        ForeColor = AppColors.TextPrimary,
        Padding = new Padding(8),
        Text = text,
        TextAlign = ContentAlignment.MiddleLeft
    };

    private void UpdateScoreAvailability()
    {
        HashSet<int> presentIds = GetPresentMemberIds().ToHashSet();
        bool editable = attestation?.Status == AttestationStatusHelper.InProgress;
        foreach (((int memberId, _), ComboBox control) in scoreControls)
        {
            bool enabled = editable && presentIds.Contains(memberId);
            control.Enabled = enabled;
            if (!presentIds.Contains(memberId)) control.SelectedIndex = 0;
        }
    }

    private void FillDecision(IReadOnlyCollection<AttestationVote> votes)
    {
        if (attestation is null) return;
        decisionTextBox.Text = attestation.Decision ?? string.Empty;
        recommendationsTextBox.Text = attestation.Recommendations ?? string.Empty;
        Dictionary<int, string> votesByMember = votes.ToDictionary(item => item.AttestationCommissionMemberId, item => item.Vote);
        votesGrid.Rows.Clear();
        foreach (AttestationCommissionMember member in members.Where(item => item.IsPresent))
        {
            int row = votesGrid.Rows.Add(member.MemberFullName, CommissionRoleHelper.GetDisplayName(member.Role), null!);
            votesGrid.Rows[row].Tag = member;
            DataGridViewComboBoxCell voteCell = (DataGridViewComboBoxCell)votesGrid.Rows[row].Cells[2];
            voteCell.DisplayMember = nameof(AttestationVoteOption.DisplayName);
            voteCell.ValueMember = nameof(AttestationVoteOption.Value);
            voteCell.DataSource = AttestationVoteHelper.AvailableVotes.ToList();
            if (votesByMember.TryGetValue(member.Id, out string? vote)) voteCell.Value = vote;
        }
    }

    private void ConfigureMode()
    {
        if (attestation is null) return;
        bool inProgress = attestation.Status == AttestationStatusHelper.InProgress;
        bool decision = attestation.Status == AttestationStatusHelper.Decision;
        bool completed = attestation.Status == AttestationStatusHelper.Completed;
        presenceList.Enabled = inProgress;
        saveScoresButton.Visible = inProgress;
        transitionButton.Visible = inProgress;
        decisionTab.Enabled = decision || completed;
        decisionTextBox.ReadOnly = completed;
        recommendationsTextBox.ReadOnly = completed;
        votesGrid.ReadOnly = completed;
        saveDecisionButton.Visible = decision;
        completeButton.Visible = decision;
        if (completed) Text = "Завершенная аттестация";
        UpdateScoreAvailability();
    }

    private void UpdateAverageLabels()
    {
        if (attestation is null) return;
        professionalAverageLabel.Text = $"Профессиональные: {FormatAverage(attestation.ProfessionalAverage)}";
        personalAverageLabel.Text = $"Личностные: {FormatAverage(attestation.PersonalAverage)}";
        managerialAverageLabel.Text = $"Руководительские: {FormatAverage(attestation.ManagerialAverage)}";
        overallAverageLabel.Text = $"Общий результат: {FormatAverage(attestation.OverallAverage)}";
    }

    private static string FormatAverage(double? value) => value.HasValue ? value.Value.ToString("0.00") : "—";

    private void SaveProgress()
    {
        if (processService is null) return;
        try
        {
            processService.SaveProgress(attestationId, GetPresentMemberIds(), GetEnteredScores());
            MessageBox.Show(this, "Оценки сохранены.", "Аттестация", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (AttestationProcessServiceException exception) { ShowError(exception.Message); }
    }

    private void TransitionToDecision()
    {
        if (processService is null) return;
        try
        {
            processService.TransitionToDecision(attestationId, GetPresentMemberIds(), GetEnteredScores());
            LoadProcess(selectDecisionTab: true);
        }
        catch (AttestationProcessServiceException exception) { ShowError(exception.Message); }
    }

    private void SaveDecision()
    {
        if (processService is null) return;
        try
        {
            processService.SaveDecision(attestationId, decisionTextBox.Text, recommendationsTextBox.Text, GetEnteredVotes());
            MessageBox.Show(this, "Решение и голоса сохранены.", "Аттестация", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (AttestationProcessServiceException exception) { ShowError(exception.Message); }
    }

    private void CompleteAttestation()
    {
        if (processService is null) return;
        if (MessageBox.Show(this, "Завершить аттестацию? После завершения данные нельзя будет изменить.",
                "Аттестация", MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2) != DialogResult.Yes) return;
        try
        {
            processService.Complete(attestationId, decisionTextBox.Text, recommendationsTextBox.Text, GetEnteredVotes());
            LoadProcess(selectDecisionTab: true);
        }
        catch (AttestationProcessServiceException exception) { ShowError(exception.Message); }
    }

    private IReadOnlyCollection<int> GetPresentMemberIds()
    {
        List<int> ids = [];
        for (int index = 0; index < members.Count && index < presenceList.Items.Count; index++)
            if (presenceList.GetItemChecked(index)) ids.Add(members[index].Id);
        return ids;
    }

    private IReadOnlyCollection<AttestationScore> GetEnteredScores()
    {
        List<AttestationScore> result = [];
        foreach (((int memberId, int criterionId), ComboBox control) in scoreControls)
        {
            if (control.Enabled && control.SelectedItem is int score)
            {
                result.Add(new AttestationScore
                {
                    AttestationId = attestationId,
                    AttestationCommissionMemberId = memberId,
                    AttestationCriterionId = criterionId,
                    Score = score
                });
            }
        }
        return result;
    }

    private IReadOnlyCollection<AttestationVote> GetEnteredVotes()
    {
        votesGrid.EndEdit();
        List<AttestationVote> result = [];
        foreach (DataGridViewRow row in votesGrid.Rows)
        {
            if (row.Tag is AttestationCommissionMember member && row.Cells[2].Value is string vote)
            {
                result.Add(new AttestationVote
                {
                    AttestationId = attestationId,
                    AttestationCommissionMemberId = member.Id,
                    Vote = vote
                });
            }
        }
        return result;
    }

    private void UpdateVoteCounts()
    {
        IReadOnlyCollection<AttestationVote> votes = GetEnteredVotes();
        int forCount = votes.Count(item => item.Vote == AttestationVoteHelper.For);
        int againstCount = votes.Count(item => item.Vote == AttestationVoteHelper.Against);
        int abstainedCount = votes.Count(item => item.Vote == AttestationVoteHelper.Abstained);
        voteCountsLabel.Text = $"За: {forCount}    Против: {againstCount}    Воздержались: {abstainedCount}";
    }

    private void ShowError(string message) => MessageBox.Show(
        this, message, "Аттестация", MessageBoxButtons.OK, MessageBoxIcon.Warning);

    private void LoadWindowIcon() { string path = Path.Combine(AppContext.BaseDirectory, "Assets", "Icons", "program-logo.ico"); if (File.Exists(path)) Icon = new Icon(path); }
}
