using CleanArchitectureCQRS.Application.DTOs;
using CleanArchitectureCQRS.Application.Queries;
using CleanArchitectureCQRS.Infrastructure.EF.Contexts;
using CleanArchitectureCQRS.Infrastructure.EF.Models;
using CleanArchitectureCQRS.Shared.Abstractions.Queries;

namespace CleanArchitectureCQRS.Infrastructure.EF.Queries.Handlers;

internal sealed class GetItemHandler : GetResourceSubtypeHandlerBase<ItemReadModel>, IQueryHandler<GetItem, ResourceSubtypeDto>
{
    public GetItemHandler(ReadDbContext context)
        : base(context.Items, item => new ResourceSubtypeDto(item.Id, item.Name))
    {
    }

    public Task<ResourceSubtypeDto?> HandleAsync(GetItem query) => GetAsync(query.Id);
}
