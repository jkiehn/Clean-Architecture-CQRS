using CleanArchitectureCQRS.Maui.Services;
using CleanArchitectureCQRS.Maui.Shared.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CleanArchitectureCQRS.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddSingleton<IFormFactor, FormFactor>();
        builder.Services.AddSingleton<ILocalePreferenceService, DeviceLocalePreferenceService>();
        builder.Services.AddHttpClient<SampleEntityService>(client =>
        {
            client.BaseAddress = new Uri(ApiBaseAddressResolver.GetDefault());
        });
        builder.Services.AddHttpClient<DescriptorEntityApiService>(client =>
        {
            client.BaseAddress = new Uri(ApiBaseAddressResolver.GetDefault());
        });
        builder.Services.AddScoped<IEntityWorkspaceService, SampleEntityWorkspaceService>();
        foreach (var descriptor in DescriptorDrivenWorkspaceDefinitions.All)
        {
            var entityDescriptor = descriptor;
            builder.Services.AddScoped<IEntityWorkspaceService>(sp =>
                new DescriptorDrivenWorkspaceService(
                    sp.GetRequiredService<DescriptorEntityApiService>(),
                    entityDescriptor));
        }
        builder.Services.AddScoped<IEntityWorkspaceRegistry, EntityWorkspaceRegistry>();
        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
