namespace EmployeeAttestation.Extra;

public static class EvaluationCategoryHelper
{
    public const string Professional = "Professional";
    public const string Personal = "Personal";
    public const string Managerial = "Managerial";

    public static IReadOnlyList<EvaluationCategoryOption> AvailableCategories { get; } =
    [
        new(Professional, "Профессиональные"),
        new(Personal, "Личностные"),
        new(Managerial, "Руководительские")
    ];

    public static string GetDisplayName(string category) => AvailableCategories
        .FirstOrDefault(item => string.Equals(item.Value, category, StringComparison.Ordinal))
        ?.DisplayName ?? category;

    public static bool IsValid(string? category) => category is not null && AvailableCategories.Any(
        item => string.Equals(item.Value, category, StringComparison.Ordinal));
}

public sealed record EvaluationCategoryOption(string Value, string DisplayName);
