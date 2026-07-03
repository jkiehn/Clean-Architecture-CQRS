using System.Net;
using System.Net.Http.Json;
using CleanArchitectureCQRS.Maui.Shared.Services;
using Shouldly;

namespace CleanArchitecture.CQRS.UnitTest.Ui;

public class AgentServiceTests
{
    [Fact]
    public async Task GetAllAsync_Uses_Agent_List_Endpoint()
    {
        var handler = new RecordingHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(Array.Empty<AgentModel>())
        });
        var service = CreateService(handler);

        await service.GetAllAsync("customer");

        handler.LastRequest.ShouldNotBeNull();
        handler.LastRequest!.Method.ShouldBe(HttpMethod.Get);
        handler.LastRequest.RequestUri!.ToString().ShouldBe("https://localhost:7256/api/Agent?searchPhrase=customer");
    }

    private static AgentService CreateService(HttpMessageHandler handler)
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
