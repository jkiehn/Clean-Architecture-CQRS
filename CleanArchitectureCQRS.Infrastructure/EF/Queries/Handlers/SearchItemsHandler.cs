using CleanArchitectureCQRS.Application.DTOs;
using CleanArchitectureCQRS.Application.Queries;
using CleanArchitectureCQRS.Infrastructure.EF.Contexts;
using CleanArchitectureCQRS.Infrastructure.EF.Models;
using CleanArchitectureCQRS.Shared.Abstractions.Queries;

namespace CleanArchitectureCQRS.Infrastructure.EF.Queries.Handlers;

internal sealed class SearchItemsHandler : SearchResourceSubtypeHandlerBase<ItemReadModel>, IQueryHandler<SearchItems, IEnumerable<ResourceSubtypeDto>>
{
    public SearchItemsHandler(ReadDbContext context)
        : base(context.Items, item => new ResourceSubtypeDto(item.Id, item.Name))
    {
    }

    public Task<IEnumerable<ResourceSubtypeDto>> HandleAsync(SearchItems query) => SearchAsync(query.SearchPhrase);
}
