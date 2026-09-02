namespace EmployeeAttestation.Models;

public sealed class EmployeeListItem
{
    public int Id { get; init; }

    public string FullName { get; init; } = string.Empty;

    public string DepartmentName { get; init; } = string.Empty;

    public string PositionName { get; init; } = string.Empty;

    public bool IsManager { get; init; }

    public bool IsArchived { get; init; }
}
