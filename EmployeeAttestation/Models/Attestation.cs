namespace EmployeeAttestation.Models;

public sealed class Attestation
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    public int CommissionId { get; set; }

    public DateTime? AttestationDate { get; set; }

    public string Status { get; set; } = string.Empty;

    public bool EvaluateManagerial { get; set; }

    public DateTime CreatedAt { get; set; }
}
