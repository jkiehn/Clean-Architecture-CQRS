namespace CleanArchitectureCQRS.Maui.Shared.Services;

public static class ApiBaseAddressResolver
{
    public const string DefaultApiBaseAddress = "https://localhost:7256/";

    public static string GetDefault()
    {
#if ANDROID
        return "https://10.0.2.2:7256/";
#else
        return DefaultApiBaseAddress;
#endif
    }
}
