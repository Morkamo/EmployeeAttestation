namespace EmployeeAttestation.Models;

public sealed class EvaluationCriterion
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int MinimumScore { get; set; } = 1;
    public int MaximumScore { get; set; } = 5;
    public bool ManagersOnly { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
}
