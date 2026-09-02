namespace EmployeeAttestation.Models;

public sealed class Department
{
    public int Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? DocumentName { get; set; }
}
