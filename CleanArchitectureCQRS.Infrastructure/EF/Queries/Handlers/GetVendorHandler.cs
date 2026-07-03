using CleanArchitectureCQRS.Application.DTOs;
using CleanArchitectureCQRS.Application.Queries;
using CleanArchitectureCQRS.Infrastructure.EF.Contexts;
using CleanArchitectureCQRS.Infrastructure.EF.Models;
using CleanArchitectureCQRS.Shared.Abstractions.Queries;

namespace CleanArchitectureCQRS.Infrastructure.EF.Queries.Handlers;

internal sealed class GetVendorHandler : GetAgentSubtypeHandlerBase<VendorReadModel>, IQueryHandler<GetVendor, AgentSubtypeDto>
{
    public GetVendorHandler(ReadDbContext context)
        : base(context.Vendors, vendor => new AgentSubtypeDto(vendor.Id, vendor.Name, vendor.Email))
    {
    }

    public Task<AgentSubtypeDto?> HandleAsync(GetVendor query) => GetAsync(query.Id);
}
