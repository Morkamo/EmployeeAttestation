namespace EmployeeAttestation.Models;

public sealed class AttestationCommissionMember
{
    public int Id { get; set; }
    public int AttestationId { get; set; }
    public int? CommissionMemberId { get; set; }
    public string MemberFullName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public bool IsPresent { get; set; } = true;

    public override string ToString() => MemberFullName;
}
