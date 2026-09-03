namespace EmployeeAttestation.Extra;

public static class AttestationVoteHelper
{
    public const string For = "For";
    public const string Against = "Against";
    public const string Abstained = "Abstained";

    public static IReadOnlyList<AttestationVoteOption> AvailableVotes { get; } =
    [
        new(For, "За"),
        new(Against, "Против"),
        new(Abstained, "Воздержался")
    ];

    public static string GetDisplayName(string vote) => AvailableVotes
        .FirstOrDefault(item => string.Equals(item.Value, vote, StringComparison.Ordinal))
        ?.DisplayName ?? vote;

    public static bool IsValid(string? vote) => vote is not null && AvailableVotes.Any(
        item => string.Equals(item.Value, vote, StringComparison.Ordinal));
}

public sealed record AttestationVoteOption(string Value, string DisplayName);
