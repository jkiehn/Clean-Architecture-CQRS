using CleanArchitectureCQRS.Domain.Entities;
using CleanArchitectureCQRS.Domain.Repositories;
using CleanArchitectureCQRS.Domain.ValueObjects;
using CleanArchitectureCQRS.Infrastructure.EF.Contexts;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureCQRS.Infrastructure.EF.Repositories;

internal class ResourceRepository<TResource> : IResourceRepository<TResource> where TResource : Resource
{
    private readonly DbSet<TResource> _resources;
    private readonly WriteDbContext _writeDbContext;

    protected ResourceRepository(WriteDbContext writeDbContext, DbSet<TResource> resources)
    {
        _resources = resources;
        _writeDbContext = writeDbContext;
    }

    public Task<TResource?> GetAsync(ResourceId id)
        => _resources.SingleOrDefaultAsync(resource => resource.Id == id);

    public async Task AddAsync(TResource resource)
    {
        await _resources.AddAsync(resource);
        await _writeDbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(TResource resource)
    {
        _resources.Update(resource);
        await _writeDbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(TResource resource)
    {
        _resources.Remove(resource);
        await _writeDbContext.SaveChangesAsync();
    }
}
