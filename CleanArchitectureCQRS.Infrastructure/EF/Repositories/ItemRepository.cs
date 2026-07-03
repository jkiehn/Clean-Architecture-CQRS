using CleanArchitectureCQRS.Domain.Entities;
using CleanArchitectureCQRS.Domain.Repositories;
using CleanArchitectureCQRS.Infrastructure.EF.Contexts;

namespace CleanArchitectureCQRS.Infrastructure.EF.Repositories;

internal sealed class ItemRepository : ResourceRepository<Item>, IItemRepository
{
    public ItemRepository(WriteDbContext writeDbContext)
        : base(writeDbContext, writeDbContext.Items)
    {
    }
}
