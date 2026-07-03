using CleanArchitectureCQRS.Domain.Entities;
using CleanArchitectureCQRS.Domain.ValueObjects;

namespace CleanArchitectureCQRS.Domain.Repositories;

public interface IResourceRepository<TResource> where TResource : Resource
{
    Task<TResource?> GetAsync(ResourceId id);
    Task AddAsync(TResource resource);
    Task UpdateAsync(TResource resource);
    Task DeleteAsync(TResource resource);
}
