namespace EmployeeAttestation.Models;

public sealed class AttestationVote
{
    public int Id { get; set; }
    public int AttestationId { get; set; }
    public int AttestationCommissionMemberId { get; set; }
    public string Vote { get; set; } = string.Empty;
}
