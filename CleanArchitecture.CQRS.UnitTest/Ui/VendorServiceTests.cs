using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using CleanArchitectureCQRS.Maui.Shared.Services;
using Shouldly;

namespace CleanArchitecture.CQRS.UnitTest.Ui;

public class VendorServiceTests
{
    [Fact]
    public async Task GetAllAsync_Uses_Vendor_List_Endpoint_With_Search_Query()
    {
        var handler = new RecordingHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(Array.Empty<AgentSubtypeModel>())
        });
        var service = CreateService(handler);

        await service.GetAllAsync("north");

        handler.LastRequest.ShouldNotBeNull();
        handler.LastRequest!.RequestUri!.ToString().ShouldBe("https://localhost:7256/api/Vendor?searchPhrase=north");
    }

    [Fact]
    public async Task UpdateAsync_Uses_Route_Id_And_Command_Body()
    {
        var handler = new RecordingHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK));
        var service = CreateService(handler);
        var vendorId = Guid.NewGuid();

        await service.UpdateAsync(new UpdateAgentSubtypeCommand(vendorId, "Northwind", "sales@northwind.example"));

        handler.LastRequest.ShouldNotBeNull();
        handler.LastRequest!.Method.ShouldBe(HttpMethod.Put);
        handler.LastRequest.RequestUri!.ToString().ShouldBe($"https://localhost:7256/api/Vendor/{vendorId}");

        var json = await handler.LastRequest.Content!.ReadAsStringAsync();
        using var document = JsonDocument.Parse(json);
        document.RootElement.GetProperty("id").GetGuid().ShouldBe(vendorId);
        document.RootElement.GetProperty("name").GetString().ShouldBe("Northwind");
        document.RootElement.GetProperty("email").GetString().ShouldBe("sales@northwind.example");
    }

    private static VendorService CreateService(HttpMessageHandler handler)
        => new(new HttpClient(handler)
        {
            BaseAddress = new Uri(ApiBaseAddressResolver.DefaultApiBaseAddress)
        });

    private sealed class RecordingHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _responseFactory;

        public HttpRequestMessage? LastRequest { get; private set; }

        public RecordingHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
        {
            _responseFactory = responseFactory;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequest = request;
            return Task.FromResult(_responseFactory(request));
        }
    }
}
