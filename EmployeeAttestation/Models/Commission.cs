namespace EmployeeAttestation.Models;

public sealed class Commission
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsArchived { get; set; }
}
