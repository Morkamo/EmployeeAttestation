namespace EmployeeAttestation.Models;

public sealed class AttestationScore
{
    public int Id { get; set; }
    public int AttestationId { get; set; }
    public int AttestationCommissionMemberId { get; set; }
    public int AttestationCriterionId { get; set; }
    public int Score { get; set; }
}
