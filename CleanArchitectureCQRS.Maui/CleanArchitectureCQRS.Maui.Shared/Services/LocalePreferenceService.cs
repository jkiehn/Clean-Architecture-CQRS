using System.Globalization;

namespace CleanArchitectureCQRS.Maui.Shared.Services;

public interface ILocalePreferenceService
{
    IReadOnlyList<LocaleOption> SupportedLocales { get; }
    string CurrentCultureName { get; }
    bool IsInitialized { get; }
    event Action? Changed;
    Task InitializeAsync();
    Task SetCultureAsync(string cultureName);
}

public sealed record LocaleOption(string CultureName, string Label);

public static class LocalePreferences
{
    public const string HeaderName = "X-Entity-Ui-Culture";
    public const string StorageKey = "entity-workspace.locale";

    public static IReadOnlyList<LocaleOption> SupportedLocales { get; } =
    [
        new("da-DK", "Dansk (Danmark)"),
        new("en-US", "English (United States)")
    ];

    public static string NormalizeCultureName(string? cultureName)
    {
        if (string.IsNullOrWhiteSpace(cultureName))
        {
            return "en-US";
        }

        var trimmed = cultureName.Trim();

        if (trimmed.StartsWith("da", StringComparison.OrdinalIgnoreCase))
        {
            return "da-DK";
        }

        if (trimmed.StartsWith("en", StringComparison.OrdinalIgnoreCase))
        {
            return "en-US";
        }

        return SupportedLocales.Any(option => string.Equals(option.CultureName, trimmed, StringComparison.OrdinalIgnoreCase))
            ? trimmed
            : "en-US";
    }

    public static void ApplyCulture(string cultureName)
    {
        var normalized = NormalizeCultureName(cultureName);
        var culture = CultureInfo.GetCultureInfo(normalized);
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
    }
}
