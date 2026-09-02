namespace EmployeeAttestation.Models;

public sealed class Position
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public bool IsManagerial { get; set; }
}
