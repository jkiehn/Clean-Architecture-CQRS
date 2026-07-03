using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using CleanArchitectureCQRS.Maui.Shared.Services;
using Shouldly;

namespace CleanArchitecture.CQRS.UnitTest.Ui;

public class ItemServiceTests
{
    [Fact]
    public async Task GetAllAsync_Uses_Item_List_Endpoint_With_Search_Query()
    {
        var handler = new RecordingHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(Array.Empty<ResourceSubtypeModel>())
        });
        var service = CreateService(handler);

        await service.GetAllAsync("widget");

        handler.LastRequest.ShouldNotBeNull();
        handler.LastRequest!.RequestUri!.ToString().ShouldBe("https://localhost:7256/api/Item?searchPhrase=widget");
    }

    [Fact]
    public async Task UpdateAsync_Uses_Route_Id_And_Command_Body()
    {
        var handler = new RecordingHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK));
        var service = CreateService(handler);
        var itemId = Guid.NewGuid();

        await service.UpdateAsync(new UpdateResourceSubtypeCommand(itemId, "Widget"));

        handler.LastRequest.ShouldNotBeNull();
        handler.LastRequest!.Method.ShouldBe(HttpMethod.Put);
        handler.LastRequest.RequestUri!.ToString().ShouldBe($"https://localhost:7256/api/Item/{itemId}");

        var json = await handler.LastRequest.Content!.ReadAsStringAsync();
        using var document = JsonDocument.Parse(json);
        document.RootElement.GetProperty("id").GetGuid().ShouldBe(itemId);
        document.RootElement.GetProperty("name").GetString().ShouldBe("Widget");
    }

    [Fact]
    public async Task DeleteAsync_Uses_Item_Delete_Endpoint()
    {
        var handler = new RecordingHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK));
        var service = CreateService(handler);
        var itemId = Guid.NewGuid();

        await service.DeleteAsync(itemId);

        handler.LastRequest.ShouldNotBeNull();
        handler.LastRequest!.Method.ShouldBe(HttpMethod.Delete);
        handler.LastRequest.RequestUri!.ToString().ShouldBe($"https://localhost:7256/api/Item/{itemId}");
    }

    private static ItemService CreateService(HttpMessageHandler handler)
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
