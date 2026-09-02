namespace EmployeeAttestation.Extra;

public static class CommissionRoleHelper
{
    public static IReadOnlyList<CommissionRoleOption> AvailableRoles { get; } =
    [
        new("Chairperson", "Председатель"),
        new("DeputyChairperson", "Заместитель председателя"),
        new("Member", "Член комиссии"),
        new("Secretary", "Секретарь")
    ];

    public static string GetDisplayName(string role) => AvailableRoles
        .FirstOrDefault(option => string.Equals(option.Value, role, StringComparison.Ordinal))
        ?.DisplayName ?? role;

    public static bool IsValidRole(string role) => AvailableRoles.Any(
        option => string.Equals(option.Value, role, StringComparison.Ordinal));
}

public sealed class CommissionRoleOption
{
    public CommissionRoleOption(string value, string displayName)
    {
        Value = value;
        DisplayName = displayName;
    }

    public string Value { get; }

    public string DisplayName { get; }
}
