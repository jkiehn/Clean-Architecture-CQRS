using CleanArchitectureCQRS.Application.DTOs;
using CleanArchitectureCQRS.Application.Queries;
using CleanArchitectureCQRS.Infrastructure.EF.Contexts;
using CleanArchitectureCQRS.Infrastructure.EF.Models;
using CleanArchitectureCQRS.Shared.Abstractions.Queries;

namespace CleanArchitectureCQRS.Infrastructure.EF.Queries.Handlers;

internal sealed class SearchVendorsHandler : SearchAgentSubtypeHandlerBase<VendorReadModel>, IQueryHandler<SearchVendors, IEnumerable<AgentSubtypeDto>>
{
    public SearchVendorsHandler(ReadDbContext context)
        : base(context.Vendors, vendor => new AgentSubtypeDto(vendor.Id, vendor.Name, vendor.Email))
    {
    }

    public Task<IEnumerable<AgentSubtypeDto>> HandleAsync(SearchVendors query) => SearchAsync(query.SearchPhrase);
}
