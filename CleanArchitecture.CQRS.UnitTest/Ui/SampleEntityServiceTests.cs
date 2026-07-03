using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using CleanArchitectureCQRS.Maui.Shared.Services;
using Shouldly;

namespace CleanArchitecture.CQRS.UnitTest.Ui;

public class SampleEntityServiceTests
{
    [Fact]
    public async Task GetAllAsync_Uses_Base_List_Endpoint_When_Search_Phrase_Is_Empty()
    {
        var handler = new RecordingHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(Array.Empty<SampleEntityModel>())
        });
        var service = CreateService(handler);

        await service.GetAllAsync();

        handler.LastRequest.ShouldNotBeNull();
        handler.LastRequest!.Method.ShouldBe(HttpMethod.Get);
        handler.LastRequest.RequestUri!.ToString().ShouldBe("https://localhost:7256/api/SampleEntity");
    }

    [Fact]
    public async Task GetAllAsync_Adds_Encoded_Search_Query_When_Search_Phrase_Is_Present()
    {
        var handler = new RecordingHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(Array.Empty<SampleEntityModel>())
        });
        var service = CreateService(handler);

        await service.GetAllAsync("blue widget");

        handler.LastRequest.ShouldNotBeNull();
        handler.LastRequest!.RequestUri!.AbsolutePath.ShouldBe("/api/SampleEntity");
        handler.LastRequest.RequestUri.Query.ShouldBe("?searchPhrase=blue%20widget");
    }

    [Fact]
    public async Task GetByIdAsync_Calls_Detail_Endpoint()
    {
        var entityId = Guid.NewGuid();
        var handler = new RecordingHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(new SampleEntityModel(entityId, "Sample", new DestinationDto("Aarhus", "Denmark"), []))
        });
        var service = CreateService(handler);

        await service.GetByIdAsync(entityId);

        handler.LastRequest.ShouldNotBeNull();
        handler.LastRequest!.Method.ShouldBe(HttpMethod.Get);
        handler.LastRequest.RequestUri!.ToString().ShouldBe($"https://localhost:7256/api/SampleEntity/{entityId}");
    }

    [Fact]
    public async Task DeleteItemAsync_Sends_Delete_To_Item_Endpoint_With_Json_Body()
    {
        var entityId = Guid.NewGuid();
        var handler = new RecordingHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK));
        var service = CreateService(handler);

        await service.DeleteItemAsync(entityId, "Item 1");

        handler.LastRequest.ShouldNotBeNull();
        handler.LastRequest!.Method.ShouldBe(HttpMethod.Delete);
        handler.LastRequest.RequestUri!.AbsolutePath
            .ShouldBe($"/api/SampleEntity/{entityId}/items/Item%201");

        var json = await handler.LastRequest.Content!.ReadAsStringAsync();
        using var document = JsonDocument.Parse(json);
        document.RootElement.GetProperty("sampleEntityId").GetGuid().ShouldBe(entityId);
        document.RootElement.GetProperty("name").GetString().ShouldBe("Item 1");
    }

    [Fact]
    public async Task DeleteAsync_Sends_Delete_To_Entity_Endpoint_With_Json_Body()
    {
        var entityId = Guid.NewGuid();
        var handler = new RecordingHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK));
        var service = CreateService(handler);

        await service.DeleteAsync(entityId);

        handler.LastRequest.ShouldNotBeNull();
        handler.LastRequest!.Method.ShouldBe(HttpMethod.Delete);
        handler.LastRequest.RequestUri!.ToString().ShouldBe($"https://localhost:7256/api/SampleEntity/{entityId}");

        var json = await handler.LastRequest.Content!.ReadAsStringAsync();
        using var document = JsonDocument.Parse(json);
        document.RootElement.GetProperty("id").GetGuid().ShouldBe(entityId);
    }

    [Fact]
    public async Task DeleteItemAsync_Throws_When_Api_Returns_Error()
    {
        var handler = new RecordingHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
            Content = new StringContent("boom", Encoding.UTF8, "text/plain")
        });
        var service = CreateService(handler);

        await Should.ThrowAsync<HttpRequestException>(() => service.DeleteItemAsync(Guid.NewGuid(), "Item 1"));
    }

    private static SampleEntityService CreateService(HttpMessageHandler handler)
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
