using CleanArchitectureCQRS.Application.Services;
using CleanArchitectureCQRS.Infrastructure.EF.Contexts;
using CleanArchitectureCQRS.Infrastructure.EF.Models;

namespace CleanArchitectureCQRS.Infrastructure.EF.Services;

internal sealed class ItemReadService : ResourceSubtypeReadService<ItemReadModel>, IItemReadService
{
    public ItemReadService(ReadDbContext context)
        : base(context.Items)
    {
    }
}
