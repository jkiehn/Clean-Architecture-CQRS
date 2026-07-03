using CleanArchitectureCQRS.Application.Services;
using CleanArchitectureCQRS.Infrastructure.EF.Models;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureCQRS.Infrastructure.EF.Services;

internal class ResourceSubtypeReadService<TReadModel> : IResourceSubtypeReadService
    where TReadModel : ResourceReadModelBase
{
    private readonly DbSet<TReadModel> _resources;

    protected ResourceSubtypeReadService(DbSet<TReadModel> resources)
    {
        _resources = resources;
    }

    public Task<bool> ExistsByNameAsync(string name, Guid? excludeResourceId = null)
        => _resources.AnyAsync(resource =>
            resource.Name == name && (!excludeResourceId.HasValue || resource.Id != excludeResourceId.Value));
}
