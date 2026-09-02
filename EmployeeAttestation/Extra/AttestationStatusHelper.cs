namespace EmployeeAttestation.Extra;

public static class AttestationStatusHelper
{
    public const string Draft = "Draft";
    public const string Scheduled = "Scheduled";
    public const string InProgress = "InProgress";
    public const string Decision = "Decision";
    public const string Completed = "Completed";
    public const string Cancelled = "Cancelled";

    private static readonly IReadOnlyDictionary<string, string> DisplayNames =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [Draft] = "Черновик",
            [Scheduled] = "Запланирована",
            [InProgress] = "Проводится",
            [Decision] = "Решение комиссии",
            [Completed] = "Завершена",
            [Cancelled] = "Отменена"
        };

    public static IReadOnlyList<string> Values { get; } =
        [Draft, Scheduled, InProgress, Decision, Completed, Cancelled];

    public static string GetDisplayName(string? status) => status is not null
        && DisplayNames.TryGetValue(status, out string? displayName)
            ? displayName
            : status ?? string.Empty;

    public static bool IsValid(string? status) => status is not null && DisplayNames.ContainsKey(status);

    public static bool CanEdit(string? status) => status is Draft or Scheduled;

    public static bool CanCancel(string? status) => status is Draft or Scheduled;
}
