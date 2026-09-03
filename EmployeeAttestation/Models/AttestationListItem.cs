namespace EmployeeAttestation.Models;

public sealed class AttestationListItem
{
    public int Id { get; init; }

    public string EmployeeFullName { get; init; } = string.Empty;

    public string DepartmentName { get; init; } = string.Empty;

    public string PositionName { get; init; } = string.Empty;

    public string CommissionName { get; init; } = string.Empty;

    public DateTime? AttestationDate { get; init; }

    public string Status { get; init; } = string.Empty;

    public bool EvaluateManagerial { get; init; }

    public double? OverallAverage { get; init; }

    public string? Decision { get; init; }
}
