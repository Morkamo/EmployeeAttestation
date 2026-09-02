namespace EmployeeAttestation.Models;

public sealed class Employee
{
    public int Id { get; set; }

    public string LastName { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string? MiddleName { get; set; }

    public int DepartmentId { get; set; }

    public int PositionId { get; set; }

    public bool IsManager { get; set; }

    public bool IsArchived { get; set; }

    public string FullName => string.Join(
        " ",
        new[] { LastName, FirstName, MiddleName }.Where(value => !string.IsNullOrWhiteSpace(value)));
}
