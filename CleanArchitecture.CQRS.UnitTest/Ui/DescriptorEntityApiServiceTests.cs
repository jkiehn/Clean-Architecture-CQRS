using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using CleanArchitectureCQRS.Maui.Shared.Services;
using Shouldly;

namespace CleanArchitecture.CQRS.UnitTest.Ui;

public class DescriptorEntityApiServiceTests
{
    [Fact]
    public async Task GetAllAsync_Uses_Generic_Entity_List_Endpoint()
    {
        var handler = new RecordingHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(Array.Empty<EntityListItem>())
        });
        var service = CreateService(handler);

        await service.GetAllAsync("customers", "alice");

        handler.LastRequest.ShouldNotBeNull();
        handler.LastRequest!.RequestUri!.ToString().ShouldBe("https://localhost:7256/api/entities/customers?searchPhrase=alice");
    }

    [Fact]
    public async Task UpdateAsync_Uses_Generic_Entity_Update_Endpoint()
    {
        var handler = new RecordingHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(new EntityOperationResult("Customer updated."))
        });
        var service = CreateService(handler);
        var entityId = Guid.NewGuid();

        await service.UpdateAsync("customers", entityId, new Dictionary<string, object?>
        {
            ["name"] = "Alice",
            ["email"] = "alice@example.com"
        });

        handler.LastRequest.ShouldNotBeNull();
        handler.LastRequest!.Method.ShouldBe(HttpMethod.Put);
        handler.LastRequest.RequestUri!.ToString().ShouldBe($"https://localhost:7256/api/entities/customers/{entityId}");

        var json = await handler.LastRequest.Content!.ReadAsStringAsync();
        using var document = JsonDocument.Parse(json);
        document.RootElement.GetProperty("values").GetProperty("name").GetString().ShouldBe("Alice");
        document.RootElement.GetProperty("values").GetProperty("email").GetString().ShouldBe("alice@example.com");
    }

    [Fact]
    public void Definitions_Expose_Current_Generic_Workspaces()
    {
        DescriptorDrivenWorkspaceDefinitions.All.Select(descriptor => descriptor.Key)
            .ShouldBe(["items", "customers", "vendors", "employees", "sales", "sales-orders", "it-contracts", "prepaid-it-report", "agents"]);

        var employeeDescriptor = DescriptorDrivenWorkspaceDefinitions.All.Single(descriptor => descriptor.Key == "employees");
        employeeDescriptor.CreateAction.ShouldNotBeNull();
        employeeDescriptor.CreateAction!.Fields!.Select(field => field.Key)
            .ShouldBe(["name", "email", "socialSecurityNumber"]);

        var saleDescriptor = DescriptorDrivenWorkspaceDefinitions.All.Single(descriptor => descriptor.Key == "sales");
        saleDescriptor.CreateAction.ShouldNotBeNull();
        saleDescriptor.CreateAction!.Fields!.Select(field => field.Key)
            .ShouldBe(["when", "endWhen", "employee", "customer"]);

        var salesOrderDescriptor = DescriptorDrivenWorkspaceDefinitions.All.Single(descriptor => descriptor.Key == "sales-orders");
        salesOrderDescriptor.CreateAction.ShouldNotBeNull();
        salesOrderDescriptor.CreateAction!.Fields!.Select(field => field.Key)
            .ShouldBe(["when", "endWhen", "employee", "customer"]);

        var itContractDescriptor = DescriptorDrivenWorkspaceDefinitions.All.Single(descriptor => descriptor.Key == "it-contracts");
        itContractDescriptor.CreateAction.ShouldNotBeNull();
        itContractDescriptor.CreateAction!.Fields!.Select(field => field.Key)
            .ShouldBe(["serviceName", "departmentCode", "startDate", "endDate", "prepaidAmount", "responsibleEmployee", "vendor"]);
    }

    private static DescriptorEntityApiService CreateService(HttpMessageHandler handler)
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
