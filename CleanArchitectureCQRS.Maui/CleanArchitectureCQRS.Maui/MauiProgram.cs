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
        builder.Services.AddHttpClient<SampleEntityService>(client =>
        {
            client.BaseAddress = new Uri(ApiBaseAddressResolver.GetDefault());
        });
        builder.Services.AddHttpClient<CustomerService>(client =>
        {
            client.BaseAddress = new Uri(ApiBaseAddressResolver.GetDefault());
        });
        builder.Services.AddHttpClient<VendorService>(client =>
        {
            client.BaseAddress = new Uri(ApiBaseAddressResolver.GetDefault());
        });
        builder.Services.AddHttpClient<AgentService>(client =>
        {
            client.BaseAddress = new Uri(ApiBaseAddressResolver.GetDefault());
        });
        builder.Services.AddScoped<IEntityWorkspaceService, SampleEntityWorkspaceService>();
        builder.Services.AddScoped<IEntityWorkspaceService, CustomerWorkspaceService>();
        builder.Services.AddScoped<IEntityWorkspaceService, VendorWorkspaceService>();
        builder.Services.AddScoped<IEntityWorkspaceService, AgentWorkspaceService>();
        builder.Services.AddScoped<IEntityWorkspaceRegistry, EntityWorkspaceRegistry>();
        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
