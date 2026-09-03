namespace EmployeeAttestation.Models;

public sealed class AttestationCriterion
{
    public int Id { get; set; }
    public int AttestationId { get; set; }
    public int? CriterionId { get; set; }
    public string CriterionCode { get; set; } = string.Empty;
    public string CriterionName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int MinimumScore { get; set; }
    public int MaximumScore { get; set; }
    public bool ManagersOnly { get; set; }
    public int SortOrder { get; set; }
}
