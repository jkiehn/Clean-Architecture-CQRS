using Microsoft.JSInterop;

namespace CleanArchitectureCQRS.Maui.Web.Services;

internal sealed class BrowserLocalePreferenceService : ILocalePreferenceService
{
    private readonly IJSRuntime _jsRuntime;

    public BrowserLocalePreferenceService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
        CurrentCultureName = LocalePreferences.NormalizeCultureName(null);
        LocalePreferences.ApplyCulture(CurrentCultureName);
    }

    public IReadOnlyList<LocaleOption> SupportedLocales => LocalePreferences.SupportedLocales;

    public string CurrentCultureName { get; private set; }

    public bool IsInitialized { get; private set; }

    public event Action? Changed;

    public async Task InitializeAsync()
    {
        if (IsInitialized)
        {
            return;
        }

        var storedCulture = await _jsRuntime.InvokeAsync<string?>("entityWorkspaceCulture.get");
        var systemCulture = string.IsNullOrWhiteSpace(storedCulture)
            ? await _jsRuntime.InvokeAsync<string?>("entityWorkspaceCulture.getSystem")
            : storedCulture;

        ApplyCulture(systemCulture);
        IsInitialized = true;
        Changed?.Invoke();
    }

    public async Task SetCultureAsync(string cultureName)
    {
        var normalized = LocalePreferences.NormalizeCultureName(cultureName);

        if (string.Equals(CurrentCultureName, normalized, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        await _jsRuntime.InvokeVoidAsync("entityWorkspaceCulture.set", normalized);
        ApplyCulture(normalized);
        Changed?.Invoke();
    }

    private void ApplyCulture(string? cultureName)
    {
        CurrentCultureName = LocalePreferences.NormalizeCultureName(cultureName);
        LocalePreferences.ApplyCulture(CurrentCultureName);
    }
}
