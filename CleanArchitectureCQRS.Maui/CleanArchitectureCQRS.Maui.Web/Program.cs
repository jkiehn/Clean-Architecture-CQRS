using CleanArchitectureCQRS.Maui.Shared.Services;
using CleanArchitectureCQRS.Maui.Web.Components;
using CleanArchitectureCQRS.Maui.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<IFormFactor, FormFactor>();
builder.Services.AddHttpClient<SampleEntityService>(client =>
{
    var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? ApiBaseAddressResolver.DefaultApiBaseAddress;
    client.BaseAddress = new Uri(apiBaseUrl);
});
builder.Services.AddHttpClient<CustomerService>(client =>
{
    var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? ApiBaseAddressResolver.DefaultApiBaseAddress;
    client.BaseAddress = new Uri(apiBaseUrl);
});
builder.Services.AddHttpClient<VendorService>(client =>
{
    var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? ApiBaseAddressResolver.DefaultApiBaseAddress;
    client.BaseAddress = new Uri(apiBaseUrl);
});
builder.Services.AddHttpClient<AgentService>(client =>
{
    var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? ApiBaseAddressResolver.DefaultApiBaseAddress;
    client.BaseAddress = new Uri(apiBaseUrl);
});
builder.Services.AddScoped<IEntityWorkspaceService, SampleEntityWorkspaceService>();
builder.Services.AddScoped<IEntityWorkspaceService, CustomerWorkspaceService>();
builder.Services.AddScoped<IEntityWorkspaceService, VendorWorkspaceService>();
builder.Services.AddScoped<IEntityWorkspaceService, AgentWorkspaceService>();
builder.Services.AddScoped<IEntityWorkspaceRegistry, EntityWorkspaceRegistry>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(
        typeof(CleanArchitectureCQRS.Maui.Shared._Imports).Assembly);

app.Run();
