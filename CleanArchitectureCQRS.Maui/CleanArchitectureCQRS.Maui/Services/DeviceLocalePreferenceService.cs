using CleanArchitectureCQRS.Maui.Shared.Services;
using System.Globalization;

namespace CleanArchitectureCQRS.Maui.Services;

internal sealed class DeviceLocalePreferenceService : ILocalePreferenceService
{
    public DeviceLocalePreferenceService()
    {
        CurrentCultureName = LocalePreferences.NormalizeCultureName(null);
        LocalePreferences.ApplyCulture(CurrentCultureName);
    }

    public IReadOnlyList<LocaleOption> SupportedLocales => LocalePreferences.SupportedLocales;

    public string CurrentCultureName { get; private set; }

    public bool IsInitialized { get; private set; }

    public event Action? Changed;

    public Task InitializeAsync()
    {
        if (IsInitialized)
        {
            return Task.CompletedTask;
        }

        var storedCulture = Preferences.Default.Get(LocalePreferences.StorageKey, string.Empty);
        var cultureName = string.IsNullOrWhiteSpace(storedCulture)
            ? CultureInfo.CurrentCulture.Name
            : storedCulture;

        ApplyCulture(cultureName);
        IsInitialized = true;
        Changed?.Invoke();
        return Task.CompletedTask;
    }

    public Task SetCultureAsync(string cultureName)
    {
        var normalized = LocalePreferences.NormalizeCultureName(cultureName);

        if (string.Equals(CurrentCultureName, normalized, StringComparison.OrdinalIgnoreCase))
        {
            return Task.CompletedTask;
        }

        Preferences.Default.Set(LocalePreferences.StorageKey, normalized);
        ApplyCulture(normalized);
        Changed?.Invoke();
        return Task.CompletedTask;
    }

    private void ApplyCulture(string? cultureName)
    {
        CurrentCultureName = LocalePreferences.NormalizeCultureName(cultureName);
        LocalePreferences.ApplyCulture(CurrentCultureName);
    }
}
