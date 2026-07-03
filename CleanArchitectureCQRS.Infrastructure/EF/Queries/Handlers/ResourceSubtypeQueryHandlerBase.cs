using System.Linq.Expressions;
using CleanArchitectureCQRS.Application.DTOs;
using CleanArchitectureCQRS.Infrastructure.EF.Models;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureCQRS.Infrastructure.EF.Queries.Handlers;

internal abstract class GetResourceSubtypeHandlerBase<TReadModel>
    where TReadModel : ResourceReadModelBase
{
    private readonly DbSet<TReadModel> _resources;
    private readonly Expression<Func<TReadModel, ResourceSubtypeDto>> _selector;

    protected GetResourceSubtypeHandlerBase(DbSet<TReadModel> resources, Expression<Func<TReadModel, ResourceSubtypeDto>> selector)
    {
        _resources = resources;
        _selector = selector;
    }

    protected Task<ResourceSubtypeDto?> GetAsync(Guid id)
        => _resources
            .Where(resource => resource.Id == id)
            .Select(_selector)
            .AsNoTracking()
            .SingleOrDefaultAsync();
}

internal abstract class SearchResourceSubtypeHandlerBase<TReadModel>
    where TReadModel : ResourceReadModelBase
{
    private readonly DbSet<TReadModel> _resources;
    private readonly Expression<Func<TReadModel, ResourceSubtypeDto>> _selector;

    protected SearchResourceSubtypeHandlerBase(DbSet<TReadModel> resources, Expression<Func<TReadModel, ResourceSubtypeDto>> selector)
    {
        _resources = resources;
        _selector = selector;
    }

    protected async Task<IEnumerable<ResourceSubtypeDto>> SearchAsync(string? searchPhrase)
    {
        var dbQuery = _resources.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchPhrase))
        {
            dbQuery = dbQuery.Where(resource =>
                Microsoft.EntityFrameworkCore.EF.Functions.Like(resource.Name, $"%{searchPhrase}%"));
        }

        return await dbQuery
            .OrderBy(resource => resource.Name)
            .Select(_selector)
            .AsNoTracking()
            .ToListAsync();
    }
}
