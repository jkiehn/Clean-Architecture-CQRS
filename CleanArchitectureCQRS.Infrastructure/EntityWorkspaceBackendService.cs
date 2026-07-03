using CleanArchitectureCQRS.Application.DTOs;
using CleanArchitectureCQRS.Application.Services;
using CleanArchitectureCQRS.Domain.Entities;
using CleanArchitectureCQRS.Domain.ValueObjects;
using CleanArchitectureCQRS.Infrastructure.EF.Contexts;
using CleanArchitectureCQRS.Infrastructure.EF.Models;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureCQRS.Infrastructure;

internal sealed class EntityWorkspaceBackendService : IEntityWorkspaceBackendService
{
    private readonly ReadDbContext _readDbContext;
    private readonly WriteDbContext _writeDbContext;
    private readonly IReadOnlyDictionary<string, EntityWorkspaceBackendDescriptor> _descriptors;

    public EntityWorkspaceBackendService(ReadDbContext readDbContext, WriteDbContext writeDbContext)
    {
        _readDbContext = readDbContext;
        _writeDbContext = writeDbContext;
        _descriptors = new[]
        {
            CreateAgentSubtypeDescriptor<Customer, CustomerReadModel>("customers", "Customer"),
            CreateAgentSubtypeDescriptor<Vendor, VendorReadModel>("vendors", "Vendor"),
            CreateEmployeeDescriptor(),
            CreateResourceSubtypeDescriptor<Item, ItemReadModel>("items", "Item"),
            CreateAgentAggregationDescriptor()
        }.ToDictionary(descriptor => descriptor.Key, StringComparer.OrdinalIgnoreCase);
    }

    public Task<IReadOnlyList<EntityWorkspaceListItemDto>> SearchAsync(string entityKey, string? searchPhrase = null)
        => GetDescriptor(entityKey).SearchAsync(this, searchPhrase);

    public Task<EntityWorkspaceDetailDto?> GetAsync(string entityKey, Guid id)
        => GetDescriptor(entityKey).GetAsync(this, id);

    public Task<EntityWorkspaceOperationResultDto> CreateAsync(string entityKey, IReadOnlyDictionary<string, string?> values)
        => GetDescriptor(entityKey).CreateAsync?.Invoke(this, values)
           ?? throw new InvalidOperationException($"{entityKey} does not support creation.");

    public Task<EntityWorkspaceOperationResultDto> UpdateAsync(string entityKey, Guid id, IReadOnlyDictionary<string, string?> values)
        => GetDescriptor(entityKey).UpdateAsync?.Invoke(this, id, values)
           ?? throw new InvalidOperationException($"{entityKey} does not support updates.");

    public Task<EntityWorkspaceOperationResultDto> DeleteAsync(string entityKey, Guid id)
        => GetDescriptor(entityKey).DeleteAsync?.Invoke(this, id)
           ?? throw new InvalidOperationException($"{entityKey} does not support deletion.");

    private EntityWorkspaceBackendDescriptor GetDescriptor(string entityKey)
    {
        if (_descriptors.TryGetValue(entityKey, out var descriptor))
        {
            return descriptor;
        }

        throw new KeyNotFoundException($"No backend workspace descriptor was registered for '{entityKey}'.");
    }

    private static EntityWorkspaceBackendDescriptor CreateAgentSubtypeDescriptor<TEntity, TReadModel>(string key, string entityType)
        where TEntity : Agent
        where TReadModel : AgentReadModelBase
        => new(
            key,
            (service, searchPhrase) => service.SearchAgentSubtypeAsync<TReadModel>(searchPhrase, entityType),
            (service, id) => service.GetAgentSubtypeAsync<TReadModel>(id, entityType),
            (service, values) => service.CreateAgentSubtypeAsync<TEntity, TReadModel>(values, entityType),
            (service, id, values) => service.UpdateAgentSubtypeAsync<TEntity, TReadModel>(id, values, entityType),
            (service, id) => service.DeleteAgentSubtypeAsync<TEntity>(id, entityType));

    private static EntityWorkspaceBackendDescriptor CreateResourceSubtypeDescriptor<TEntity, TReadModel>(string key, string entityType)
        where TEntity : Resource
        where TReadModel : ResourceReadModelBase
        => new(
            key,
            (service, searchPhrase) => service.SearchResourceSubtypeAsync<TReadModel>(searchPhrase, entityType),
            (service, id) => service.GetResourceSubtypeAsync<TReadModel>(id, entityType),
            (service, values) => service.CreateResourceSubtypeAsync<TEntity, TReadModel>(values, entityType),
            (service, id, values) => service.UpdateResourceSubtypeAsync<TEntity, TReadModel>(id, values, entityType),
            (service, id) => service.DeleteResourceSubtypeAsync<TEntity>(id, entityType));

    private static EntityWorkspaceBackendDescriptor CreateEmployeeDescriptor()
        => new(
            "employees",
            (service, searchPhrase) => service.SearchEmployeesAsync(searchPhrase),
            (service, id) => service.GetEmployeeAsync(id),
            (service, values) => service.CreateEmployeeAsync(values),
            (service, id, values) => service.UpdateEmployeeAsync(id, values),
            (service, id) => service.DeleteAgentSubtypeAsync<Employee>(id, "Employee"));

    private static EntityWorkspaceBackendDescriptor CreateAgentAggregationDescriptor()
        => new(
            "agents",
            (service, searchPhrase) => service.SearchAgentsAsync(searchPhrase),
            (service, id) => service.GetAgentAsync(id),
            null,
            null,
            null);

    private async Task<IReadOnlyList<EntityWorkspaceListItemDto>> SearchAgentSubtypeAsync<TReadModel>(string? searchPhrase, string entityType)
        where TReadModel : AgentReadModelBase
    {
        var query = _readDbContext.Set<TReadModel>().AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchPhrase))
        {
            query = query.Where(entity =>
                Microsoft.EntityFrameworkCore.EF.Functions.Like(entity.Name, $"%{searchPhrase}%") ||
                Microsoft.EntityFrameworkCore.EF.Functions.Like(entity.Email, $"%{searchPhrase}%"));
        }

        return await query
            .OrderBy(entity => entity.Name)
            .Select(entity => new EntityWorkspaceListItemDto(
                entity.Id,
                entity.Name,
                entity.Email,
                new[]
                {
                    new EntityWorkspacePropertyDto("Type", entityType)
                }))
            .AsNoTracking()
            .ToListAsync();
    }

    private async Task<IReadOnlyList<EntityWorkspaceListItemDto>> SearchEmployeesAsync(string? searchPhrase)
    {
        var query = _readDbContext.Employees.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchPhrase))
        {
            query = query.Where(employee =>
                Microsoft.EntityFrameworkCore.EF.Functions.Like(employee.Name, $"%{searchPhrase}%") ||
                Microsoft.EntityFrameworkCore.EF.Functions.Like(employee.Email, $"%{searchPhrase}%") ||
                Microsoft.EntityFrameworkCore.EF.Functions.Like(employee.SocialSecurityNumber, $"%{searchPhrase}%"));
        }

        return await query
            .OrderBy(employee => employee.Name)
            .Select(employee => new EntityWorkspaceListItemDto(
                employee.Id,
                employee.Name,
                employee.Email,
                new[]
                {
                    new EntityWorkspacePropertyDto("Type", "Employee"),
                    new EntityWorkspacePropertyDto("SSN", employee.SocialSecurityNumber)
                }))
            .AsNoTracking()
            .ToListAsync();
    }

    private async Task<EntityWorkspaceDetailDto?> GetAgentSubtypeAsync<TReadModel>(Guid id, string entityType)
        where TReadModel : AgentReadModelBase
    {
        var entity = await _readDbContext.Set<TReadModel>()
            .Where(item => item.Id == id)
            .Select(item => new { item.Id, item.Name, item.Email })
            .AsNoTracking()
            .SingleOrDefaultAsync();

        return entity is null
            ? null
            : new EntityWorkspaceDetailDto(
                entity.Id,
                entity.Name,
                entity.Email,
                new[]
                {
                    new EntityWorkspacePropertyDto("Email", entity.Email),
                    new EntityWorkspacePropertyDto("Identifier", entity.Id.ToString()),
                    new EntityWorkspacePropertyDto("Type", entityType)
                },
                new Dictionary<string, object?>
                {
                    ["name"] = entity.Name,
                    ["email"] = entity.Email
                },
                Array.Empty<EntityWorkspaceCollectionSectionDto>());
    }

    private async Task<EntityWorkspaceDetailDto?> GetEmployeeAsync(Guid id)
    {
        var employee = await _readDbContext.Employees
            .Where(item => item.Id == id)
            .Select(item => new { item.Id, item.Name, item.Email, item.SocialSecurityNumber })
            .AsNoTracking()
            .SingleOrDefaultAsync();

        return employee is null
            ? null
            : new EntityWorkspaceDetailDto(
                employee.Id,
                employee.Name,
                employee.Email,
                new[]
                {
                    new EntityWorkspacePropertyDto("Email", employee.Email),
                    new EntityWorkspacePropertyDto("Social Security Number", employee.SocialSecurityNumber),
                    new EntityWorkspacePropertyDto("Identifier", employee.Id.ToString()),
                    new EntityWorkspacePropertyDto("Type", "Employee")
                },
                new Dictionary<string, object?>
                {
                    ["name"] = employee.Name,
                    ["email"] = employee.Email,
                    ["socialSecurityNumber"] = employee.SocialSecurityNumber
                },
                Array.Empty<EntityWorkspaceCollectionSectionDto>());
    }

    private async Task<EntityWorkspaceOperationResultDto> CreateAgentSubtypeAsync<TEntity, TReadModel>(IReadOnlyDictionary<string, string?> values, string entityType)
        where TEntity : Agent
        where TReadModel : AgentReadModelBase
    {
        var name = GetRequiredValue(values, "name");
        var email = GetRequiredValue(values, "email");

        if (await _readDbContext.Set<TReadModel>().AnyAsync(item => item.Email == email))
        {
            throw new InvalidOperationException($"{entityType} with email '{email}' already exists.");
        }

        var id = Guid.NewGuid();
        var entity = CreateEntityInstance<TEntity>(id, name, email);

        await _writeDbContext.Set<TEntity>().AddAsync(entity);
        await _writeDbContext.SaveChangesAsync();
        return new EntityWorkspaceOperationResultDto($"{entityType} created.", id);
    }

    private async Task<EntityWorkspaceOperationResultDto> UpdateAgentSubtypeAsync<TEntity, TReadModel>(Guid id, IReadOnlyDictionary<string, string?> values, string entityType)
        where TEntity : Agent
        where TReadModel : AgentReadModelBase
    {
        var entity = await _writeDbContext.Set<TEntity>().SingleOrDefaultAsync(item => item.Id == (AgentId)id);

        if (entity is null)
        {
            throw new InvalidOperationException($"{entityType} with ID '{id}' was not found.");
        }

        var name = GetRequiredValue(values, "name");
        var email = GetRequiredValue(values, "email");

        if (await _readDbContext.Set<TReadModel>().AnyAsync(item => item.Email == email && item.Id != id))
        {
            throw new InvalidOperationException($"{entityType} with email '{email}' already exists.");
        }

        entity.UpdateDetails(name, email);
        await _writeDbContext.SaveChangesAsync();
        return new EntityWorkspaceOperationResultDto($"{entityType} updated.", id);
    }

    private async Task<EntityWorkspaceOperationResultDto> CreateEmployeeAsync(IReadOnlyDictionary<string, string?> values)
    {
        var name = GetRequiredValue(values, "name");
        var email = GetRequiredValue(values, "email");
        var socialSecurityNumber = GetRequiredValue(values, "socialSecurityNumber");

        if (await _readDbContext.Employees.AnyAsync(employee => employee.Email == email))
        {
            throw new InvalidOperationException($"Employee with email '{email}' already exists.");
        }

        if (await _readDbContext.Employees.AnyAsync(employee => employee.SocialSecurityNumber == socialSecurityNumber))
        {
            throw new InvalidOperationException($"Employee with social security number '{socialSecurityNumber}' already exists.");
        }

        var id = Guid.NewGuid();
        var employee = CreateEntityInstance<Employee>(id, name, email, socialSecurityNumber);

        await _writeDbContext.Employees.AddAsync(employee);
        await _writeDbContext.SaveChangesAsync();
        return new EntityWorkspaceOperationResultDto("Employee created.", id);
    }

    private async Task<EntityWorkspaceOperationResultDto> UpdateEmployeeAsync(Guid id, IReadOnlyDictionary<string, string?> values)
    {
        var employee = await _writeDbContext.Employees.SingleOrDefaultAsync(item => item.Id == (AgentId)id);

        if (employee is null)
        {
            throw new InvalidOperationException($"Employee with ID '{id}' was not found.");
        }

        var name = GetRequiredValue(values, "name");
        var email = GetRequiredValue(values, "email");
        var socialSecurityNumber = GetRequiredValue(values, "socialSecurityNumber");

        if (await _readDbContext.Employees.AnyAsync(item => item.Email == email && item.Id != id))
        {
            throw new InvalidOperationException($"Employee with email '{email}' already exists.");
        }

        if (await _readDbContext.Employees.AnyAsync(item => item.SocialSecurityNumber == socialSecurityNumber && item.Id != id))
        {
            throw new InvalidOperationException($"Employee with social security number '{socialSecurityNumber}' already exists.");
        }

        employee.UpdateDetails(name, email, socialSecurityNumber);
        await _writeDbContext.SaveChangesAsync();
        return new EntityWorkspaceOperationResultDto("Employee updated.", id);
    }

    private async Task<EntityWorkspaceOperationResultDto> DeleteAgentSubtypeAsync<TEntity>(Guid id, string entityType)
        where TEntity : Agent
    {
        var entity = await _writeDbContext.Set<TEntity>().SingleOrDefaultAsync(item => item.Id == (AgentId)id);

        if (entity is null)
        {
            throw new InvalidOperationException($"{entityType} with ID '{id}' was not found.");
        }

        _writeDbContext.Set<TEntity>().Remove(entity);
        await _writeDbContext.SaveChangesAsync();
        return new EntityWorkspaceOperationResultDto($"{entityType} deleted.");
    }

    private async Task<IReadOnlyList<EntityWorkspaceListItemDto>> SearchResourceSubtypeAsync<TReadModel>(string? searchPhrase, string entityType)
        where TReadModel : ResourceReadModelBase
    {
        var query = _readDbContext.Set<TReadModel>().AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchPhrase))
        {
            query = query.Where(entity => Microsoft.EntityFrameworkCore.EF.Functions.Like(entity.Name, $"%{searchPhrase}%"));
        }

        return await query
            .OrderBy(entity => entity.Name)
            .Select(entity => new EntityWorkspaceListItemDto(
                entity.Id,
                entity.Name,
                null,
                new[]
                {
                    new EntityWorkspacePropertyDto("Type", entityType)
                }))
            .AsNoTracking()
            .ToListAsync();
    }

    private async Task<EntityWorkspaceDetailDto?> GetResourceSubtypeAsync<TReadModel>(Guid id, string entityType)
        where TReadModel : ResourceReadModelBase
    {
        var entity = await _readDbContext.Set<TReadModel>()
            .Where(item => item.Id == id)
            .Select(item => new { item.Id, item.Name })
            .AsNoTracking()
            .SingleOrDefaultAsync();

        return entity is null
            ? null
            : new EntityWorkspaceDetailDto(
                entity.Id,
                entity.Name,
                null,
                new[]
                {
                    new EntityWorkspacePropertyDto("Identifier", entity.Id.ToString()),
                    new EntityWorkspacePropertyDto("Type", entityType)
                },
                new Dictionary<string, object?>
                {
                    ["name"] = entity.Name
                },
                Array.Empty<EntityWorkspaceCollectionSectionDto>());
    }

    private async Task<EntityWorkspaceOperationResultDto> CreateResourceSubtypeAsync<TEntity, TReadModel>(IReadOnlyDictionary<string, string?> values, string entityType)
        where TEntity : Resource
        where TReadModel : ResourceReadModelBase
    {
        var name = GetRequiredValue(values, "name");

        if (await _readDbContext.Set<TReadModel>().AnyAsync(item => item.Name == name))
        {
            throw new InvalidOperationException($"{entityType} with name '{name}' already exists.");
        }

        var id = Guid.NewGuid();
        var entity = CreateEntityInstance<TEntity>(id, name);

        await _writeDbContext.Set<TEntity>().AddAsync(entity);
        await _writeDbContext.SaveChangesAsync();
        return new EntityWorkspaceOperationResultDto($"{entityType} created.", id);
    }

    private async Task<EntityWorkspaceOperationResultDto> UpdateResourceSubtypeAsync<TEntity, TReadModel>(Guid id, IReadOnlyDictionary<string, string?> values, string entityType)
        where TEntity : Resource
        where TReadModel : ResourceReadModelBase
    {
        var entity = await _writeDbContext.Set<TEntity>().SingleOrDefaultAsync(item => item.Id == (ResourceId)id);

        if (entity is null)
        {
            throw new InvalidOperationException($"{entityType} with ID '{id}' was not found.");
        }

        var name = GetRequiredValue(values, "name");

        if (await _readDbContext.Set<TReadModel>().AnyAsync(item => item.Name == name && item.Id != id))
        {
            throw new InvalidOperationException($"{entityType} with name '{name}' already exists.");
        }

        entity.UpdateDetails(name);
        await _writeDbContext.SaveChangesAsync();
        return new EntityWorkspaceOperationResultDto($"{entityType} updated.", id);
    }

    private async Task<EntityWorkspaceOperationResultDto> DeleteResourceSubtypeAsync<TEntity>(Guid id, string entityType)
        where TEntity : Resource
    {
        var entity = await _writeDbContext.Set<TEntity>().SingleOrDefaultAsync(item => item.Id == (ResourceId)id);

        if (entity is null)
        {
            throw new InvalidOperationException($"{entityType} with ID '{id}' was not found.");
        }

        _writeDbContext.Set<TEntity>().Remove(entity);
        await _writeDbContext.SaveChangesAsync();
        return new EntityWorkspaceOperationResultDto($"{entityType} deleted.");
    }

    private async Task<IReadOnlyList<EntityWorkspaceListItemDto>> SearchAgentsAsync(string? searchPhrase)
    {
        var customers = await SearchAgentSubtypeAsync<CustomerReadModel>(searchPhrase, "Customer");
        var vendors = await SearchAgentSubtypeAsync<VendorReadModel>(searchPhrase, "Vendor");
        var employees = await SearchEmployeesAsync(searchPhrase);

        return customers
            .Concat(vendors)
            .Concat(employees)
            .OrderBy(item => item.Title)
            .ToArray();
    }

    private async Task<EntityWorkspaceDetailDto?> GetAgentAsync(Guid id)
    {
        var customer = await GetAgentSubtypeAsync<CustomerReadModel>(id, "Customer");
        var vendor = customer is null ? await GetAgentSubtypeAsync<VendorReadModel>(id, "Vendor") : null;
        return vendor ?? customer ?? await GetEmployeeAsync(id);
    }

    private static string GetRequiredValue(IReadOnlyDictionary<string, string?> values, string key)
    {
        if (!values.TryGetValue(key, out var value) || string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"Field '{key}' is required.");
        }

        return value.Trim();
    }

    private static TEntity CreateEntityInstance<TEntity>(params object[] arguments)
    {
        var constructors = typeof(TEntity).GetConstructors();

        foreach (var constructor in constructors)
        {
            var parameters = constructor.GetParameters();

            if (parameters.Length != arguments.Length)
            {
                continue;
            }

            var convertedArguments = new object?[arguments.Length];
            var canConvert = true;

            for (var i = 0; i < parameters.Length; i++)
            {
                if (!TryConvertArgument(arguments[i], parameters[i].ParameterType, out var convertedArgument))
                {
                    canConvert = false;
                    break;
                }

                convertedArguments[i] = convertedArgument;
            }

            if (!canConvert)
            {
                continue;
            }

            return (TEntity)constructor.Invoke(convertedArguments);
        }

        throw new InvalidOperationException($"Could not construct {typeof(TEntity).Name}.");
    }

    private static bool TryConvertArgument(object argument, Type targetType, out object? convertedArgument)
    {
        if (targetType.IsInstanceOfType(argument))
        {
            convertedArgument = argument;
            return true;
        }

        if (argument is Guid guid)
        {
            if (targetType == typeof(AgentId))
            {
                convertedArgument = new AgentId(guid);
                return true;
            }

            if (targetType == typeof(ResourceId))
            {
                convertedArgument = new ResourceId(guid);
                return true;
            }

            if (targetType == typeof(EventId))
            {
                convertedArgument = new EventId(guid);
                return true;
            }
        }

        if (argument is string value)
        {
            if (targetType == typeof(AgentName))
            {
                convertedArgument = new AgentName(value);
                return true;
            }

            if (targetType == typeof(AgentEmail))
            {
                convertedArgument = new AgentEmail(value);
                return true;
            }

            if (targetType == typeof(ResourceName))
            {
                convertedArgument = new ResourceName(value);
                return true;
            }
        }

        convertedArgument = null;
        return false;
    }
}

internal sealed record EntityWorkspaceBackendDescriptor(
    string Key,
    Func<EntityWorkspaceBackendService, string?, Task<IReadOnlyList<EntityWorkspaceListItemDto>>> SearchAsync,
    Func<EntityWorkspaceBackendService, Guid, Task<EntityWorkspaceDetailDto?>> GetAsync,
    Func<EntityWorkspaceBackendService, IReadOnlyDictionary<string, string?>, Task<EntityWorkspaceOperationResultDto>>? CreateAsync,
    Func<EntityWorkspaceBackendService, Guid, IReadOnlyDictionary<string, string?>, Task<EntityWorkspaceOperationResultDto>>? UpdateAsync,
    Func<EntityWorkspaceBackendService, Guid, Task<EntityWorkspaceOperationResultDto>>? DeleteAsync);
