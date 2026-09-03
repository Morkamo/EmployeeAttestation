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

    public double? ProfessionalAverage { get; set; }
    public double? PersonalAverage { get; set; }
    public double? ManagerialAverage { get; set; }
    public double? OverallAverage { get; set; }
    public string? Decision { get; set; }
    public string? Recommendations { get; set; }
    public int? CommissionMembersCount { get; set; }
    public int? PresentMembersCount { get; set; }
    public int? VotesFor { get; set; }
    public int? VotesAgainst { get; set; }
    public int? VotesAbstained { get; set; }
    public DateTime? CompletedAt { get; set; }

    public string EmployeeFullName { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public string PositionName { get; set; } = string.Empty;
    public string CommissionName { get; set; } = string.Empty;
}
