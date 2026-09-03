namespace EmployeeAttestation.Models;

public sealed class AttestationDocumentData
{
    public required Attestation Attestation { get; init; }

    public required IReadOnlyList<AttestationCriterion> Criteria { get; init; }

    public required IReadOnlyList<AttestationCommissionMember> CommissionMembers { get; init; }

    public required IReadOnlyList<AttestationScore> Scores { get; init; }

    public required IReadOnlyList<AttestationVote> Votes { get; init; }
}
