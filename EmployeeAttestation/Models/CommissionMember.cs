namespace EmployeeAttestation.Models;

public sealed class CommissionMember
{
    public int Id { get; set; }

    public string LastName { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string? MiddleName { get; set; }

    public bool IsArchived { get; set; }

    public string FullName => string.Join(
        " ",
        new[] { LastName, FirstName, MiddleName }.Where(value => !string.IsNullOrWhiteSpace(value)));
}
