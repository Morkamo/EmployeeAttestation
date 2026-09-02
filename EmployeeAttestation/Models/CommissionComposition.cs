namespace EmployeeAttestation.Models;

public sealed class CommissionComposition
{
    public int Id { get; set; }

    public int CommissionId { get; set; }

    public int CommissionMemberId { get; set; }

    public string Role { get; set; } = string.Empty;

    public int SortOrder { get; set; }

    public string CommissionMemberName { get; set; } = string.Empty;
}
