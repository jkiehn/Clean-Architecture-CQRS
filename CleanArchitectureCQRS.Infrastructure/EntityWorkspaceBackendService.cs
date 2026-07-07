using CleanArchitectureCQRS.Application.DTOs;
using CleanArchitectureCQRS.Application.Services;
using CleanArchitectureCQRS.Domain.Entities;
using CleanArchitectureCQRS.Domain.ValueObjects;
using CleanArchitectureCQRS.Infrastructure.EF.Contexts;
using CleanArchitectureCQRS.Infrastructure.EF.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace CleanArchitectureCQRS.Infrastructure;

internal sealed class EntityWorkspaceBackendService : IEntityWorkspaceBackendService
{
    private static readonly Guid PrepaidItExpenseCalculatedReportId = new("9D8F7E27-56AF-4A41-9965-65B02689AE43");
    private static readonly Guid PrepaidItExpenseThirtyDayReportId = new("B7F44E88-7479-40F6-9664-8C1FAFBF6A13");

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
            CreateSaleDescriptor(),
            CreateSalesOrderDescriptor(),
            CreateItContractDescriptor(),
            CreatePrepaidItReportDescriptor(),
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

    public Task<EntityWorkspaceOperationResultDto> ExecuteCollectionActionAsync(string entityKey, Guid id, string collectionKey, string actionKey, IReadOnlyDictionary<string, string?> values)
        => GetDescriptor(entityKey).ExecuteCollectionActionAsync?.Invoke(this, id, collectionKey, actionKey, values)
           ?? throw new InvalidOperationException($"{entityKey} does not support collection actions.");

    public Task<EntityWorkspaceOperationResultDto> ExecuteCollectionItemActionAsync(string entityKey, Guid id, string collectionKey, string itemKey, string actionKey)
        => GetDescriptor(entityKey).ExecuteCollectionItemActionAsync?.Invoke(this, id, collectionKey, itemKey, actionKey)
           ?? throw new InvalidOperationException($"{entityKey} does not support collection item actions.");

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

    private static EntityWorkspaceBackendDescriptor CreateSaleDescriptor()
        => new(
            "sales",
            (service, searchPhrase) => service.SearchSalesAsync(searchPhrase),
            (service, id) => service.GetSaleAsync(id),
            (service, values) => service.CreateSaleAsync(values),
            (service, id, values) => service.UpdateSaleAsync(id, values),
            (service, id) => service.DeleteSaleAsync(id),
            (service, id, collectionKey, actionKey, values) => service.ExecuteSaleCollectionActionAsync(id, collectionKey, actionKey, values),
            (service, id, collectionKey, itemKey, actionKey) => service.ExecuteSaleCollectionItemActionAsync(id, collectionKey, itemKey, actionKey));

    private static EntityWorkspaceBackendDescriptor CreateSalesOrderDescriptor()
        => new(
            "sales-orders",
            (service, searchPhrase) => service.SearchSalesOrdersAsync(searchPhrase),
            (service, id) => service.GetSalesOrderAsync(id),
            (service, values) => service.CreateSalesOrderAsync(values),
            (service, id, values) => service.UpdateSalesOrderAsync(id, values),
            (service, id) => service.DeleteSalesOrderAsync(id),
            (service, id, collectionKey, actionKey, values) => service.ExecuteSalesOrderCollectionActionAsync(id, collectionKey, actionKey, values),
            (service, id, collectionKey, itemKey, actionKey) => service.ExecuteSalesOrderCollectionItemActionAsync(id, collectionKey, itemKey, actionKey));

    private static EntityWorkspaceBackendDescriptor CreateItContractDescriptor()
        => new(
            "it-contracts",
            (service, searchPhrase) => service.SearchItContractsAsync(searchPhrase),
            (service, id) => service.GetItContractAsync(id),
            (service, values) => service.CreateItContractAsync(values),
            (service, id, values) => service.UpdateItContractAsync(id, values),
            (service, id) => service.DeleteItContractAsync(id));

    private static EntityWorkspaceBackendDescriptor CreatePrepaidItReportDescriptor()
        => new(
            "prepaid-it-report",
            (service, searchPhrase) => service.SearchPrepaidItReportAsync(searchPhrase),
            (service, id) => service.GetPrepaidItReportAsync(id),
            null,
            null,
            null,
            (service, id, collectionKey, actionKey, values) => service.ExecutePrepaidItReportCollectionActionAsync(id, collectionKey, actionKey, values));

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

    private async Task<IReadOnlyList<EntityWorkspaceListItemDto>> SearchSalesAsync(string? searchPhrase)
    {
        var search = searchPhrase?.Trim();
        var hasAmountFilter = decimal.TryParse(search, out var parsedAmount);

        var query =
            from sale in _readDbContext.Sales
            join employee in _readDbContext.Employees on sale.EmployeeId equals employee.Id
            join customer in _readDbContext.Customers on sale.CustomerId equals customer.Id
            select new
            {
                sale.Id,
                sale.When,
                sale.Amount,
                EmployeeName = employee.Name,
                EmployeeEmail = employee.Email,
                CustomerName = customer.Name,
                CustomerEmail = customer.Email
            };

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(item =>
                Microsoft.EntityFrameworkCore.EF.Functions.Like(item.EmployeeName, $"%{search}%") ||
                Microsoft.EntityFrameworkCore.EF.Functions.Like(item.EmployeeEmail, $"%{search}%") ||
                Microsoft.EntityFrameworkCore.EF.Functions.Like(item.CustomerName, $"%{search}%") ||
                Microsoft.EntityFrameworkCore.EF.Functions.Like(item.CustomerEmail, $"%{search}%") ||
                (hasAmountFilter && item.Amount == parsedAmount));
        }

        return await query
            .OrderByDescending(item => item.When)
            .Select(item => new EntityWorkspaceListItemDto(
                item.Id,
                $"Sale {FormatDateTime(item.When)}",
                $"{item.EmployeeName} -> {item.CustomerName}",
                new[]
                {
                    new EntityWorkspacePropertyDto("Type", "Sale"),
                    new EntityWorkspacePropertyDto("Amount", FormatAmount(item.Amount))
                }))
            .AsNoTracking()
            .ToListAsync();
    }

    private async Task<EntityWorkspaceDetailDto?> GetSaleAsync(Guid id)
    {
        var sale =
            await (
                from entity in _readDbContext.Sales
                join employee in _readDbContext.Employees on entity.EmployeeId equals employee.Id
                join customer in _readDbContext.Customers on entity.CustomerId equals customer.Id
                where entity.Id == id
                select new
                {
                    entity.Id,
                    entity.When,
                    entity.EndWhen,
                    entity.Amount,
                    entity.InternalParticipationId,
                    entity.ExternalParticipationId,
                    EmployeeName = employee.Name,
                    EmployeeEmail = employee.Email,
                    CustomerName = customer.Name,
                    CustomerEmail = customer.Email
                })
            .AsNoTracking()
            .SingleOrDefaultAsync();

        return sale is null
            ? null
            : new EntityWorkspaceDetailDto(
                sale.Id,
                $"Sale {FormatDateTime(sale.When)}",
                $"{sale.EmployeeName} -> {sale.CustomerName}",
                new[]
                {
                    new EntityWorkspacePropertyDto("When", FormatDateTime(sale.When)),
                    new EntityWorkspacePropertyDto("End When", FormatOptionalDateTime(sale.EndWhen)),
                    new EntityWorkspacePropertyDto("Amount", FormatAmount(sale.Amount)),
                    new EntityWorkspacePropertyDto("Employee", $"{sale.EmployeeName} ({sale.EmployeeEmail})"),
                    new EntityWorkspacePropertyDto("Customer", $"{sale.CustomerName} ({sale.CustomerEmail})"),
                    new EntityWorkspacePropertyDto("Internal Participation", sale.InternalParticipationId.ToString()),
                    new EntityWorkspacePropertyDto("External Participation", sale.ExternalParticipationId.ToString()),
                    new EntityWorkspacePropertyDto("Identifier", sale.Id.ToString()),
                    new EntityWorkspacePropertyDto("Type", "Sale")
                },
                new Dictionary<string, object?>
                {
                    ["when"] = sale.When.ToString("O"),
                    ["endWhen"] = sale.EndWhen?.ToString("O") ?? string.Empty,
                    ["employee"] = sale.EmployeeEmail,
                    ["customer"] = sale.CustomerEmail
                },
                new[]
                {
                    await BuildSalesLineCollectionAsync(sale.Id)
                });
    }

    private async Task<EntityWorkspaceOperationResultDto> CreateSaleAsync(IReadOnlyDictionary<string, string?> values)
    {
        var when = ParseRequiredDateTimeOffset(values, "when");
        var endWhen = ParseOptionalDateTimeOffset(values, "endWhen");
        var employee = await ResolveEmployeeAsync(GetRequiredValue(values, "employee"));
        var customer = await ResolveCustomerAsync(GetRequiredValue(values, "customer"));

        var id = Guid.NewGuid();
        var sale = new Sale(id, when, employee.Id, customer.Id, endWhen);

        await _writeDbContext.Sales.AddAsync(sale);
        await _writeDbContext.SaveChangesAsync();
        return new EntityWorkspaceOperationResultDto("Sale created.", id);
    }

    private async Task<EntityWorkspaceOperationResultDto> UpdateSaleAsync(Guid id, IReadOnlyDictionary<string, string?> values)
    {
        var sale = await _writeDbContext.Sales.SingleOrDefaultAsync(item => item.Id == (EventId)id);

        if (sale is null)
        {
            throw new InvalidOperationException($"Sale with ID '{id}' was not found.");
        }

        var when = ParseRequiredDateTimeOffset(values, "when");
        var endWhen = ParseOptionalDateTimeOffset(values, "endWhen");
        var employee = await ResolveEmployeeAsync(GetRequiredValue(values, "employee"));
        var customer = await ResolveCustomerAsync(GetRequiredValue(values, "customer"));

        sale.UpdateDetails(when, employee.Id, customer.Id, endWhen);
        await _writeDbContext.SaveChangesAsync();
        return new EntityWorkspaceOperationResultDto("Sale updated.", id);
    }

    private async Task<EntityWorkspaceOperationResultDto> DeleteSaleAsync(Guid id)
    {
        var sale = await _writeDbContext.Sales.SingleOrDefaultAsync(item => item.Id == (EventId)id);

        if (sale is null)
        {
            throw new InvalidOperationException($"Sale with ID '{id}' was not found.");
        }

        _writeDbContext.Sales.Remove(sale);
        await _writeDbContext.SaveChangesAsync();
        return new EntityWorkspaceOperationResultDto("Sale deleted.");
    }

    private async Task<EntityWorkspaceCollectionSectionDto> BuildSalesLineCollectionAsync(Guid saleId)
    {
        var lineData =
            await (
                from line in _readDbContext.SalesLines
                join item in _readDbContext.Items on line.ItemId equals item.Id
                where line.SaleId == saleId
                orderby item.Name, line.Id
                select new
                {
                    line.Id,
                    item.Name,
                    line.Quantity,
                    line.UnitPrice
                })
            .AsNoTracking()
            .ToListAsync();

        var lines = lineData
            .Select(line => new EntityWorkspaceCollectionItemDto(
                line.Id.ToString(),
                line.Name,
                $"Qty {FormatNumber(line.Quantity)} @ {FormatAmount(line.UnitPrice)}",
                new[]
                {
                    new EntityWorkspacePropertyDto("Line Total", FormatAmount(line.UnitPrice * line.Quantity)),
                    new EntityWorkspacePropertyDto("Identifier", line.Id.ToString())
                },
                new[]
                {
                    new EntityWorkspaceCollectionItemActionDto("remove", "Remove", "btn btn-sm btn-outline-danger")
                }))
            .ToList();

        return new EntityWorkspaceCollectionSectionDto(
            "salesLines",
            "Sales Lines",
            "No sales lines yet. Add the first line below.",
            lines,
            new EntityWorkspaceActionDefinitionDto(
                "add",
                "Add line",
                "btn btn-primary action-btn",
                new[]
                {
                    new EntityWorkspaceFieldDefinitionDto(
                        "item",
                        "Item",
                        Required: true,
                        Placeholder: "Item name or ID",
                        Lookup: new EntityWorkspaceLookupDefinitionDto("items")),
                    new EntityWorkspaceFieldDefinitionDto(
                        "quantity",
                        "Quantity",
                        EntityWorkspaceFieldKindDto.Number,
                        Required: true,
                        Placeholder: "1",
                        DefaultValue: 1m),
                    new EntityWorkspaceFieldDefinitionDto(
                        "unitPrice",
                        "Unit Price",
                        EntityWorkspaceFieldKindDto.Number,
                        Required: true,
                        Placeholder: "100.00")
                }));
    }

    private async Task<EntityWorkspaceOperationResultDto> ExecuteSaleCollectionActionAsync(Guid saleId, string collectionKey, string actionKey, IReadOnlyDictionary<string, string?> values)
    {
        if (!string.Equals(collectionKey, "salesLines", StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(actionKey, "add", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"Unsupported sale collection action '{collectionKey}/{actionKey}'.");
        }

        var sale = await _writeDbContext.Sales
            .Include("_salesLines")
            .SingleOrDefaultAsync(item => item.Id == (EventId)saleId);

        if (sale is null)
        {
            throw new InvalidOperationException($"Sale with ID '{saleId}' was not found.");
        }

        var item = await ResolveItemAsync(GetRequiredValue(values, "item"));
        var quantity = ParseRequiredDecimal(values, "quantity");
        var unitPrice = ParseRequiredDecimal(values, "unitPrice");
        var line = sale.AddSalesLine(item.Id, unitPrice, quantity);

        await _writeDbContext.SaveChangesAsync();
        return new EntityWorkspaceOperationResultDto("Sales line added.", line.Id.Value);
    }

    private async Task<EntityWorkspaceOperationResultDto> ExecuteSaleCollectionItemActionAsync(Guid saleId, string collectionKey, string itemKey, string actionKey)
    {
        if (!string.Equals(collectionKey, "salesLines", StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(actionKey, "remove", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"Unsupported sale collection item action '{collectionKey}/{actionKey}'.");
        }

        if (!Guid.TryParse(itemKey, out var lineId))
        {
            throw new InvalidOperationException($"Sales line key '{itemKey}' is invalid.");
        }

        var sale = await _writeDbContext.Sales
            .Include("_salesLines")
            .SingleOrDefaultAsync(item => item.Id == (EventId)saleId);

        if (sale is null)
        {
            throw new InvalidOperationException($"Sale with ID '{saleId}' was not found.");
        }

        sale.RemoveSalesLine(new StockflowId(lineId));
        await _writeDbContext.SaveChangesAsync();
        return new EntityWorkspaceOperationResultDto("Sales line removed.");
    }

    private async Task<IReadOnlyList<EntityWorkspaceListItemDto>> SearchSalesOrdersAsync(string? searchPhrase)
    {
        var search = searchPhrase?.Trim();
        var hasAmountFilter = decimal.TryParse(search, out var parsedAmount);

        var query =
            from salesOrder in _readDbContext.SalesOrders
            join employee in _readDbContext.Employees on salesOrder.EmployeeId equals employee.Id
            join customer in _readDbContext.Customers on salesOrder.CustomerId equals customer.Id
            select new
            {
                salesOrder.Id,
                salesOrder.When,
                salesOrder.Amount,
                EmployeeName = employee.Name,
                EmployeeEmail = employee.Email,
                CustomerName = customer.Name,
                CustomerEmail = customer.Email
            };

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(item =>
                Microsoft.EntityFrameworkCore.EF.Functions.Like(item.EmployeeName, $"%{search}%") ||
                Microsoft.EntityFrameworkCore.EF.Functions.Like(item.EmployeeEmail, $"%{search}%") ||
                Microsoft.EntityFrameworkCore.EF.Functions.Like(item.CustomerName, $"%{search}%") ||
                Microsoft.EntityFrameworkCore.EF.Functions.Like(item.CustomerEmail, $"%{search}%") ||
                (hasAmountFilter && item.Amount == parsedAmount));
        }

        return await query
            .OrderByDescending(item => item.When)
            .Select(item => new EntityWorkspaceListItemDto(
                item.Id,
                $"Sales order {FormatDateTime(item.When)}",
                $"{item.EmployeeName} -> {item.CustomerName}",
                new[]
                {
                    new EntityWorkspacePropertyDto("Type", "Sales Order"),
                    new EntityWorkspacePropertyDto("Amount", FormatAmount(item.Amount))
                }))
            .AsNoTracking()
            .ToListAsync();
    }

    private async Task<EntityWorkspaceDetailDto?> GetSalesOrderAsync(Guid id)
    {
        var salesOrder =
            await (
                from entity in _readDbContext.SalesOrders
                join employee in _readDbContext.Employees on entity.EmployeeId equals employee.Id
                join customer in _readDbContext.Customers on entity.CustomerId equals customer.Id
                where entity.Id == id
                select new
                {
                    entity.Id,
                    entity.When,
                    entity.EndWhen,
                    entity.Amount,
                    entity.InternalParticipationId,
                    entity.ExternalParticipationId,
                    EmployeeName = employee.Name,
                    EmployeeEmail = employee.Email,
                    CustomerName = customer.Name,
                    CustomerEmail = customer.Email
                })
            .AsNoTracking()
            .SingleOrDefaultAsync();

        return salesOrder is null
            ? null
            : new EntityWorkspaceDetailDto(
                salesOrder.Id,
                $"Sales order {FormatDateTime(salesOrder.When)}",
                $"{salesOrder.EmployeeName} -> {salesOrder.CustomerName}",
                new[]
                {
                    new EntityWorkspacePropertyDto("When", FormatDateTime(salesOrder.When)),
                    new EntityWorkspacePropertyDto("End When", FormatOptionalDateTime(salesOrder.EndWhen)),
                    new EntityWorkspacePropertyDto("Amount", FormatAmount(salesOrder.Amount)),
                    new EntityWorkspacePropertyDto("Employee", $"{salesOrder.EmployeeName} ({salesOrder.EmployeeEmail})"),
                    new EntityWorkspacePropertyDto("Customer", $"{salesOrder.CustomerName} ({salesOrder.CustomerEmail})"),
                    new EntityWorkspacePropertyDto("Internal Participation", salesOrder.InternalParticipationId.ToString()),
                    new EntityWorkspacePropertyDto("External Participation", salesOrder.ExternalParticipationId.ToString()),
                    new EntityWorkspacePropertyDto("Identifier", salesOrder.Id.ToString()),
                    new EntityWorkspacePropertyDto("Type", "Sales Order")
                },
                new Dictionary<string, object?>
                {
                    ["when"] = salesOrder.When.ToString("O"),
                    ["endWhen"] = salesOrder.EndWhen?.ToString("O") ?? string.Empty,
                    ["employee"] = salesOrder.EmployeeEmail,
                    ["customer"] = salesOrder.CustomerEmail
                },
                new[]
                {
                    await BuildSalesOrderLineCollectionAsync(salesOrder.Id)
                });
    }

    private async Task<EntityWorkspaceOperationResultDto> CreateSalesOrderAsync(IReadOnlyDictionary<string, string?> values)
    {
        var when = ParseRequiredDateTimeOffset(values, "when");
        var endWhen = ParseOptionalDateTimeOffset(values, "endWhen");
        var employee = await ResolveEmployeeAsync(GetRequiredValue(values, "employee"));
        var customer = await ResolveCustomerAsync(GetRequiredValue(values, "customer"));

        var id = Guid.NewGuid();
        var salesOrder = new SalesOrder(id, when, employee.Id, customer.Id, endWhen);

        await _writeDbContext.SalesOrders.AddAsync(salesOrder);
        await _writeDbContext.SaveChangesAsync();
        return new EntityWorkspaceOperationResultDto("Sales order created.", id);
    }

    private async Task<EntityWorkspaceOperationResultDto> UpdateSalesOrderAsync(Guid id, IReadOnlyDictionary<string, string?> values)
    {
        var salesOrder = await _writeDbContext.SalesOrders.SingleOrDefaultAsync(item => item.Id == (CommitmentId)id);

        if (salesOrder is null)
        {
            throw new InvalidOperationException($"Sales order with ID '{id}' was not found.");
        }

        var when = ParseRequiredDateTimeOffset(values, "when");
        var endWhen = ParseOptionalDateTimeOffset(values, "endWhen");
        var employee = await ResolveEmployeeAsync(GetRequiredValue(values, "employee"));
        var customer = await ResolveCustomerAsync(GetRequiredValue(values, "customer"));

        salesOrder.UpdateDetails(when, employee.Id, customer.Id, endWhen);
        await _writeDbContext.SaveChangesAsync();
        return new EntityWorkspaceOperationResultDto("Sales order updated.", id);
    }

    private async Task<EntityWorkspaceOperationResultDto> DeleteSalesOrderAsync(Guid id)
    {
        var salesOrder = await _writeDbContext.SalesOrders.SingleOrDefaultAsync(item => item.Id == (CommitmentId)id);

        if (salesOrder is null)
        {
            throw new InvalidOperationException($"Sales order with ID '{id}' was not found.");
        }

        _writeDbContext.SalesOrders.Remove(salesOrder);
        await _writeDbContext.SaveChangesAsync();
        return new EntityWorkspaceOperationResultDto("Sales order deleted.");
    }

    private async Task<EntityWorkspaceCollectionSectionDto> BuildSalesOrderLineCollectionAsync(Guid salesOrderId)
    {
        var lineData =
            await (
                from line in _readDbContext.SalesOrderLines
                join item in _readDbContext.Items on line.ItemId equals item.Id
                where line.SalesOrderId == salesOrderId
                orderby item.Name, line.Id
                select new
                {
                    line.Id,
                    item.Name,
                    line.Quantity,
                    line.UnitPrice
                })
            .AsNoTracking()
            .ToListAsync();

        var lines = lineData
            .Select(line => new EntityWorkspaceCollectionItemDto(
                line.Id.ToString(),
                line.Name,
                $"Qty {FormatNumber(line.Quantity)} @ {FormatAmount(line.UnitPrice)}",
                new[]
                {
                    new EntityWorkspacePropertyDto("Line Total", FormatAmount(line.UnitPrice * line.Quantity)),
                    new EntityWorkspacePropertyDto("Identifier", line.Id.ToString())
                },
                new[]
                {
                    new EntityWorkspaceCollectionItemActionDto("remove", "Remove", "btn btn-sm btn-outline-danger")
                }))
            .ToList();

        return new EntityWorkspaceCollectionSectionDto(
            "salesOrderLines",
            "Sales Order Lines",
            "No sales order lines yet. Add the first line below.",
            lines,
            new EntityWorkspaceActionDefinitionDto(
                "add",
                "Add line",
                "btn btn-primary action-btn",
                new[]
                {
                    new EntityWorkspaceFieldDefinitionDto(
                        "item",
                        "Item",
                        Required: true,
                        Placeholder: "Item name or ID",
                        Lookup: new EntityWorkspaceLookupDefinitionDto("items")),
                    new EntityWorkspaceFieldDefinitionDto(
                        "quantity",
                        "Quantity",
                        EntityWorkspaceFieldKindDto.Number,
                        Required: true,
                        Placeholder: "1",
                        DefaultValue: 1m),
                    new EntityWorkspaceFieldDefinitionDto(
                        "unitPrice",
                        "Unit Price",
                        EntityWorkspaceFieldKindDto.Number,
                        Required: true,
                        Placeholder: "100.00")
                }));
    }

    private async Task<EntityWorkspaceOperationResultDto> ExecuteSalesOrderCollectionActionAsync(Guid salesOrderId, string collectionKey, string actionKey, IReadOnlyDictionary<string, string?> values)
    {
        if (!string.Equals(collectionKey, "salesOrderLines", StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(actionKey, "add", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"Unsupported sales order collection action '{collectionKey}/{actionKey}'.");
        }

        var salesOrder = await _writeDbContext.SalesOrders
            .Include("_salesOrderLines")
            .SingleOrDefaultAsync(item => item.Id == (CommitmentId)salesOrderId);

        if (salesOrder is null)
        {
            throw new InvalidOperationException($"Sales order with ID '{salesOrderId}' was not found.");
        }

        var item = await ResolveItemAsync(GetRequiredValue(values, "item"));
        var quantity = ParseRequiredDecimal(values, "quantity");
        var unitPrice = ParseRequiredDecimal(values, "unitPrice");
        var line = salesOrder.AddSalesOrderLine(item.Id, unitPrice, quantity);

        await _writeDbContext.SaveChangesAsync();
        return new EntityWorkspaceOperationResultDto("Sales order line added.", line.Id.Value);
    }

    private async Task<EntityWorkspaceOperationResultDto> ExecuteSalesOrderCollectionItemActionAsync(Guid salesOrderId, string collectionKey, string itemKey, string actionKey)
    {
        if (!string.Equals(collectionKey, "salesOrderLines", StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(actionKey, "remove", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"Unsupported sales order collection item action '{collectionKey}/{actionKey}'.");
        }

        if (!Guid.TryParse(itemKey, out var lineId))
        {
            throw new InvalidOperationException($"Sales order line key '{itemKey}' is invalid.");
        }

        var salesOrder = await _writeDbContext.SalesOrders
            .Include("_salesOrderLines")
            .SingleOrDefaultAsync(item => item.Id == (CommitmentId)salesOrderId);

        if (salesOrder is null)
        {
            throw new InvalidOperationException($"Sales order with ID '{salesOrderId}' was not found.");
        }

        salesOrder.RemoveSalesOrderLine(new StockflowId(lineId));
        await _writeDbContext.SaveChangesAsync();
        return new EntityWorkspaceOperationResultDto("Sales order line removed.");
    }

    private async Task<IReadOnlyList<EntityWorkspaceListItemDto>> SearchItContractsAsync(string? searchPhrase)
    {
        var search = searchPhrase?.Trim();
        var hasAmountFilter = decimal.TryParse(search, out var parsedAmount);

        var query =
            from contract in _readDbContext.ItContracts
            join employee in _readDbContext.Employees on contract.ResponsibleEmployeeId equals employee.Id
            join vendor in _readDbContext.Vendors on contract.VendorId equals vendor.Id
            select new
            {
                contract.Id,
                contract.ServiceName,
                contract.DepartmentCode,
                contract.When,
                contract.EndWhen,
                contract.Amount,
                EmployeeName = employee.Name,
                VendorName = vendor.Name
            };

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(item =>
                Microsoft.EntityFrameworkCore.EF.Functions.Like(item.ServiceName, $"%{search}%") ||
                Microsoft.EntityFrameworkCore.EF.Functions.Like(item.DepartmentCode, $"%{search}%") ||
                Microsoft.EntityFrameworkCore.EF.Functions.Like(item.EmployeeName, $"%{search}%") ||
                Microsoft.EntityFrameworkCore.EF.Functions.Like(item.VendorName, $"%{search}%") ||
                (hasAmountFilter && item.Amount == parsedAmount));
        }

        return await query
            .OrderBy(item => item.DepartmentCode)
            .ThenBy(item => item.ServiceName)
            .Select(item => new EntityWorkspaceListItemDto(
                item.Id,
                item.ServiceName,
                $"{item.DepartmentCode} · {item.VendorName}",
                new[]
                {
                    new EntityWorkspacePropertyDto("Type", "IT Contract"),
                    new EntityWorkspacePropertyDto("Amount", FormatAmount(item.Amount)),
                    new EntityWorkspacePropertyDto("Range", $"{FormatDateOnly(item.When)} to {FormatDateOnly(item.EndWhen)}")
                }))
            .AsNoTracking()
            .ToListAsync();
    }

    private async Task<EntityWorkspaceDetailDto?> GetItContractAsync(Guid id)
    {
        var contract =
            await (
                from entity in _readDbContext.ItContracts
                join employee in _readDbContext.Employees on entity.ResponsibleEmployeeId equals employee.Id
                join vendor in _readDbContext.Vendors on entity.VendorId equals vendor.Id
                where entity.Id == id
                select new
                {
                    entity.Id,
                    entity.ServiceName,
                    entity.DepartmentCode,
                    entity.When,
                    entity.EndWhen,
                    entity.Amount,
                    entity.ResponsibleEmployeeId,
                    entity.VendorId,
                    entity.InternalParticipationId,
                    entity.ExternalParticipationId,
                    EmployeeName = employee.Name,
                    EmployeeEmail = employee.Email,
                    VendorName = vendor.Name,
                    VendorEmail = vendor.Email
                })
            .AsNoTracking()
            .SingleOrDefaultAsync();

        if (contract is null)
        {
            return null;
        }

        var domainContract = new ItContract(
            contract.Id,
            contract.ServiceName,
            contract.When,
            contract.EndWhen!.Value,
            contract.Amount ?? 0m,
            contract.DepartmentCode,
            new AgentId(contract.ResponsibleEmployeeId),
            new AgentId(contract.VendorId));
        var monthlyExpenses = domainContract.CalculateMonthlyExpenses();

        return new EntityWorkspaceDetailDto(
            contract.Id,
            contract.ServiceName,
            $"{contract.DepartmentCode} · {contract.VendorName}",
            new[]
            {
                new EntityWorkspacePropertyDto("Department", contract.DepartmentCode),
                new EntityWorkspacePropertyDto("Vendor", $"{contract.VendorName} ({contract.VendorEmail})"),
                new EntityWorkspacePropertyDto("Responsible Employee", $"{contract.EmployeeName} ({contract.EmployeeEmail})"),
                new EntityWorkspacePropertyDto("Start Date", FormatDateOnly(contract.When)),
                new EntityWorkspacePropertyDto("End Date", FormatDateOnly(contract.EndWhen)),
                new EntityWorkspacePropertyDto("Prepaid Amount", FormatAmount(contract.Amount)),
                new EntityWorkspacePropertyDto("Internal Participation", contract.InternalParticipationId.ToString()),
                new EntityWorkspacePropertyDto("External Participation", contract.ExternalParticipationId.ToString()),
                new EntityWorkspacePropertyDto("Identifier", contract.Id.ToString()),
                new EntityWorkspacePropertyDto("Type", "IT Contract")
            },
            new Dictionary<string, object?>
            {
                ["serviceName"] = contract.ServiceName,
                ["departmentCode"] = contract.DepartmentCode,
                ["vendor"] = contract.VendorEmail,
                ["responsibleEmployee"] = contract.EmployeeEmail,
                ["startDate"] = contract.When.ToString("O"),
                ["endDate"] = contract.EndWhen?.ToString("O") ?? string.Empty,
                ["prepaidAmount"] = contract.Amount
            },
            new[]
            {
                BuildMonthlyExpenseCollection("monthlyExpenses", "Monthly Expense Allocation", monthlyExpenses)
            });
    }

    private async Task<EntityWorkspaceOperationResultDto> CreateItContractAsync(IReadOnlyDictionary<string, string?> values)
    {
        var serviceName = GetRequiredValue(values, "serviceName");
        var departmentCode = GetRequiredValue(values, "departmentCode");
        var startDate = ParseRequiredDateTimeOffset(values, "startDate");
        var endDate = ParseRequiredDateTimeOffset(values, "endDate");
        var prepaidAmount = ParseRequiredDecimal(values, "prepaidAmount");
        var employee = await ResolveEmployeeAsync(GetRequiredValue(values, "responsibleEmployee"));
        var vendor = await ResolveVendorAsync(GetRequiredValue(values, "vendor"));

        var id = Guid.NewGuid();
        var contract = new ItContract(id, serviceName, startDate, endDate, prepaidAmount, departmentCode, employee.Id, vendor.Id);

        await _writeDbContext.ItContracts.AddAsync(contract);
        await _writeDbContext.SaveChangesAsync();
        return new EntityWorkspaceOperationResultDto("IT contract created.", id);
    }

    private async Task<EntityWorkspaceOperationResultDto> UpdateItContractAsync(Guid id, IReadOnlyDictionary<string, string?> values)
    {
        var contract = await _writeDbContext.ItContracts.SingleOrDefaultAsync(item => item.Id == (CommitmentId)id);

        if (contract is null)
        {
            throw new InvalidOperationException($"IT contract with ID '{id}' was not found.");
        }

        var serviceName = GetRequiredValue(values, "serviceName");
        var departmentCode = GetRequiredValue(values, "departmentCode");
        var startDate = ParseRequiredDateTimeOffset(values, "startDate");
        var endDate = ParseRequiredDateTimeOffset(values, "endDate");
        var prepaidAmount = ParseRequiredDecimal(values, "prepaidAmount");
        var employee = await ResolveEmployeeAsync(GetRequiredValue(values, "responsibleEmployee"));
        var vendor = await ResolveVendorAsync(GetRequiredValue(values, "vendor"));

        contract.UpdateDetails(serviceName, startDate, endDate, prepaidAmount, departmentCode, employee.Id, vendor.Id);
        await _writeDbContext.SaveChangesAsync();
        return new EntityWorkspaceOperationResultDto("IT contract updated.", id);
    }

    private async Task<EntityWorkspaceOperationResultDto> DeleteItContractAsync(Guid id)
    {
        var contract = await _writeDbContext.ItContracts.SingleOrDefaultAsync(item => item.Id == (CommitmentId)id);

        if (contract is null)
        {
            throw new InvalidOperationException($"IT contract with ID '{id}' was not found.");
        }

        _writeDbContext.ItContracts.Remove(contract);
        await _writeDbContext.SaveChangesAsync();
        return new EntityWorkspaceOperationResultDto("IT contract deleted.");
    }

    private Task<IReadOnlyList<EntityWorkspaceListItemDto>> SearchPrepaidItReportAsync(string? searchPhrase)
    {
        IReadOnlyList<EntityWorkspaceListItemDto> items =
        [
            new EntityWorkspaceListItemDto(
                PrepaidItExpenseCalculatedReportId,
                "Prepaid IT Department Report",
                "Monthly department expense totals across all active IT contracts",
                new[]
                {
                    new EntityWorkspacePropertyDto("Type", "Report")
                })
        ];

        return Task.FromResult(items);
    }

    private async Task<EntityWorkspaceDetailDto?> GetPrepaidItReportAsync(Guid id)
    {
        if (!TryGetDaysInMonthForReportId(id, out var daysInMonth))
        {
            return null;
        }

        var contracts = await (
            from contract in _readDbContext.ItContracts
            join employee in _readDbContext.Employees on contract.ResponsibleEmployeeId equals employee.Id
            join vendor in _readDbContext.Vendors on contract.VendorId equals vendor.Id
            select new ItContractReportContractRow(
                contract.Id,
                contract.ServiceName,
                contract.DepartmentCode,
                contract.When,
                contract.EndWhen,
                contract.Amount,
                contract.ResponsibleEmployeeId,
                contract.VendorId,
                employee.Email,
                vendor.Email))
            .AsNoTracking()
            .ToListAsync();

        var monthlyExpenses = contracts
            .SelectMany(contract => new ItContract(
                    contract.Id,
                    contract.ServiceName,
                    contract.When,
                    contract.EndWhen!.Value,
                    contract.Amount ?? 0m,
                    contract.DepartmentCode,
                    new AgentId(contract.EmployeeId),
                    new AgentId(contract.VendorId))
                .CalculateMonthlyExpenses(daysInMonth))
            .GroupBy(expense => new { expense.DepartmentCode, expense.Month })
            .Select(group => new ContractMonthlyExpense(
                group.Key.Month,
                group.Key.DepartmentCode,
                group.Sum(item => item.Amount),
                group.Sum(item => item.CoveredDays),
                group.Average(item => item.PricePerDay)))
            .OrderBy(item => item.Month)
            .ThenBy(item => item.DepartmentCode)
            .ToList();

        var mermaidDiagram = BuildDepartmentExpenseMermaid(monthlyExpenses);

        return new EntityWorkspaceDetailDto(
            id,
            "Prepaid IT Department Report",
            "Monthly department expense totals across all IT contracts",
            new[]
            {
                new EntityWorkspacePropertyDto("Days In Month", FormatDaysInMonth(daysInMonth)),
                new EntityWorkspacePropertyDto("Contracts Included", contracts.Count.ToString(CultureInfo.InvariantCulture)),
                new EntityWorkspacePropertyDto("Department Count", monthlyExpenses.Select(item => item.DepartmentCode).Distinct(StringComparer.OrdinalIgnoreCase).Count().ToString(CultureInfo.InvariantCulture)),
                new EntityWorkspacePropertyDto("Month Count", monthlyExpenses.Select(item => item.Month).Distinct().Count().ToString(CultureInfo.InvariantCulture)),
                new EntityWorkspacePropertyDto("Mermaid Diagram", mermaidDiagram)
            },
            new Dictionary<string, object?>(),
            new[]
            {
                BuildPrepaidItReportOptionsCollection(daysInMonth),
                BuildMonthlyExpenseCollection("departmentMonthlyExpenses", "Department Monthly Expenses", monthlyExpenses),
                BuildContractCollection("contracts", "Contracts Included", contracts)
            });
    }

    private Task<EntityWorkspaceOperationResultDto> ExecutePrepaidItReportCollectionActionAsync(Guid id, string collectionKey, string actionKey, IReadOnlyDictionary<string, string?> values)
    {
        if (!TryGetDaysInMonthForReportId(id, out _))
        {
            throw new InvalidOperationException("The requested prepaid IT report was not found.");
        }

        if (!string.Equals(collectionKey, "reportOptions", StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(actionKey, "generate", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("The requested prepaid IT report action is not supported.");
        }

        var daysInMonth = ParseDaysInMonth(values);
        var reportId = GetPrepaidItReportId(daysInMonth);
        return Task.FromResult(new EntityWorkspaceOperationResultDto(
            $"Prepaid IT report recalculated using {FormatDaysInMonth(daysInMonth)}.",
            reportId));
    }

    private static EntityWorkspaceCollectionSectionDto BuildPrepaidItReportOptionsCollection(DaysInMonth daysInMonth)
    {
        return new EntityWorkspaceCollectionSectionDto(
            "reportOptions",
            "Calculation Options",
            "Choose how month lengths should be interpreted when allocating prepaid contract expenses.",
            new[]
            {
                new EntityWorkspaceCollectionItemDto(
                    "current",
                    "Current Report Settings",
                    "The selected option is used for every contract in this report run.",
                    new[]
                    {
                        new EntityWorkspacePropertyDto("Days In Month", FormatDaysInMonth(daysInMonth))
                    },
                    Array.Empty<EntityWorkspaceCollectionItemActionDto>())
            },
            new EntityWorkspaceActionDefinitionDto(
                "generate",
                "Generate Report",
                "btn btn-dark action-btn",
                new[]
                {
                    new EntityWorkspaceFieldDefinitionDto(
                        "daysInMonth",
                        "Days In Month",
                        EntityWorkspaceFieldKindDto.Select,
                        true,
                        DefaultValue: ToDaysInMonthValue(daysInMonth),
                        Options:
                        [
                            new EntityWorkspaceFieldOptionDto(ToDaysInMonthValue(DaysInMonth.Calculated), "Calculated"),
                            new EntityWorkspaceFieldOptionDto(ToDaysInMonthValue(DaysInMonth.ThirtyDays), "30 days")
                        ])
                }));
    }

    private static EntityWorkspaceCollectionSectionDto BuildMonthlyExpenseCollection(string key, string title, IReadOnlyList<ContractMonthlyExpense> monthlyExpenses)
    {
        var items = monthlyExpenses
            .Select(expense => new EntityWorkspaceCollectionItemDto(
                $"{expense.DepartmentCode}-{expense.Month:yyyy-MM}",
                $"{expense.DepartmentCode} · {expense.Month:yyyy-MM}",
                $"Covered {expense.CoveredDays} day(s)",
                new[]
                {
                    new EntityWorkspacePropertyDto("Amount", expense.Amount.ToString("0.00", CultureInfo.InvariantCulture)),
                    new EntityWorkspacePropertyDto("Price/Day", expense.PricePerDay.ToString("0.########", CultureInfo.InvariantCulture))
                },
                Array.Empty<EntityWorkspaceCollectionItemActionDto>()))
            .ToList();

        return new EntityWorkspaceCollectionSectionDto(
            key,
            title,
            "No monthly expense rows are available.",
            items);
    }

    private static EntityWorkspaceCollectionSectionDto BuildContractCollection(string key, string title, IReadOnlyList<ItContractReportContractRow> contracts)
    {
        var items = contracts
            .Select(contract => new EntityWorkspaceCollectionItemDto(
                contract.Id.ToString(),
                contract.ServiceName,
                $"{contract.DepartmentCode} · {FormatDateOnly(contract.When)} to {FormatDateOnly(contract.EndWhen)}",
                new[]
                {
                    new EntityWorkspacePropertyDto("Prepaid Amount", FormatAmount(contract.Amount)),
                    new EntityWorkspacePropertyDto("Identifier", contract.Id.ToString())
                },
                Array.Empty<EntityWorkspaceCollectionItemActionDto>()))
            .ToList();

        return new EntityWorkspaceCollectionSectionDto(
            key,
            title,
            "No IT contracts are currently available.",
            items);
    }

    private static string BuildDepartmentExpenseMermaid(IReadOnlyList<ContractMonthlyExpense> monthlyExpenses)
    {
        if (monthlyExpenses.Count == 0)
        {
            return "xychart-beta\n    title \"No prepaid IT expenses\"\n    x-axis []\n    y-axis \"Amount\" 0 --> 0";
        }

        var months = monthlyExpenses
            .Select(item => item.Month)
            .Distinct()
            .OrderBy(month => month)
            .ToList();

        var departments = monthlyExpenses
            .Select(item => item.DepartmentCode)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(code => code)
            .ToList();

        var maxAmount = monthlyExpenses.Max(item => item.Amount);
        var lines = new List<string>
        {
            "xychart-beta",
            "    title \"Prepaid IT Expenses by Department\"",
            $"    x-axis [{string.Join(", ", months.Select(month => $"\"{month:yyyy-MM}\""))}]",
            $"    y-axis \"Amount\" 0 --> {Math.Ceiling(maxAmount)}"
        };

        foreach (var department in departments)
        {
            var series = months
                .Select(month => monthlyExpenses
                    .Where(item => item.DepartmentCode.Equals(department, StringComparison.OrdinalIgnoreCase) && item.Month == month)
                    .Sum(item => item.Amount)
                    .ToString("0.00", CultureInfo.InvariantCulture));

            lines.Add($"    line \"{department}\" [{string.Join(", ", series)}]");
        }

        return string.Join(Environment.NewLine, lines);
    }

    private static string ToDaysInMonthValue(DaysInMonth daysInMonth)
        => daysInMonth switch
        {
            DaysInMonth.Calculated => "calculated",
            DaysInMonth.ThirtyDays => "30days",
            _ => throw new InvalidOperationException("Unsupported days-in-month option.")
        };

    private static DaysInMonth ParseDaysInMonth(IReadOnlyDictionary<string, string?> values)
    {
        var value = GetRequiredValue(values, "daysInMonth");
        return value.Trim().ToLowerInvariant() switch
        {
            "calculated" => DaysInMonth.Calculated,
            "30days" => DaysInMonth.ThirtyDays,
            _ => throw new InvalidOperationException($"DaysInMonth value '{value}' is not supported.")
        };
    }

    private static bool TryGetDaysInMonthForReportId(Guid id, out DaysInMonth daysInMonth)
    {
        if (id == PrepaidItExpenseCalculatedReportId)
        {
            daysInMonth = DaysInMonth.Calculated;
            return true;
        }

        if (id == PrepaidItExpenseThirtyDayReportId)
        {
            daysInMonth = DaysInMonth.ThirtyDays;
            return true;
        }

        daysInMonth = default;
        return false;
    }

    private static Guid GetPrepaidItReportId(DaysInMonth daysInMonth)
        => daysInMonth switch
        {
            DaysInMonth.Calculated => PrepaidItExpenseCalculatedReportId,
            DaysInMonth.ThirtyDays => PrepaidItExpenseThirtyDayReportId,
            _ => throw new InvalidOperationException("Unsupported days-in-month option.")
        };

    private static string FormatDaysInMonth(DaysInMonth daysInMonth)
        => daysInMonth switch
        {
            DaysInMonth.Calculated => "Calculated",
            DaysInMonth.ThirtyDays => "30 days",
            _ => throw new InvalidOperationException("Unsupported days-in-month option.")
        };

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

    private async Task<(AgentId Id, string Name, string Email)> ResolveEmployeeAsync(string value)
    {
        var search = value.Trim();
        var hasId = Guid.TryParse(search, out var id);

        var matches = await _readDbContext.Employees
            .Where(employee =>
                (hasId && employee.Id == id) ||
                employee.Email == search ||
                employee.Name == search ||
                employee.SocialSecurityNumber == search)
            .Select(employee => new { employee.Id, employee.Name, employee.Email })
            .AsNoTracking()
            .ToListAsync();

        if (matches.Count == 0)
        {
            throw new InvalidOperationException($"Employee '{search}' was not found.");
        }

        if (matches.Count > 1)
        {
            throw new InvalidOperationException($"Employee reference '{search}' is ambiguous. Use email, ID, or social security number.");
        }

        var match = matches[0];
        return (new AgentId(match.Id), match.Name, match.Email);
    }

    private async Task<(AgentId Id, string Name, string Email)> ResolveCustomerAsync(string value)
    {
        var search = value.Trim();
        var hasId = Guid.TryParse(search, out var id);

        var matches = await _readDbContext.Customers
            .Where(customer =>
                (hasId && customer.Id == id) ||
                customer.Email == search ||
                customer.Name == search)
            .Select(customer => new { customer.Id, customer.Name, customer.Email })
            .AsNoTracking()
            .ToListAsync();

        if (matches.Count == 0)
        {
            throw new InvalidOperationException($"Customer '{search}' was not found.");
        }

        if (matches.Count > 1)
        {
            throw new InvalidOperationException($"Customer reference '{search}' is ambiguous. Use email or ID.");
        }

        var match = matches[0];
        return (new AgentId(match.Id), match.Name, match.Email);
    }

    private async Task<(ResourceId Id, string Name)> ResolveItemAsync(string value)
    {
        var search = value.Trim();
        var hasId = Guid.TryParse(search, out var id);

        var matches = await _readDbContext.Items
            .Where(item =>
                (hasId && item.Id == id) ||
                item.Name == search)
            .Select(item => new { item.Id, item.Name })
            .AsNoTracking()
            .ToListAsync();

        if (matches.Count == 0)
        {
            throw new InvalidOperationException($"Item '{search}' was not found.");
        }

        if (matches.Count > 1)
        {
            throw new InvalidOperationException($"Item reference '{search}' is ambiguous. Use the lookup or item ID.");
        }

        var match = matches[0];
        return (new ResourceId(match.Id), match.Name);
    }

    private async Task<(AgentId Id, string Name, string Email)> ResolveVendorAsync(string value)
    {
        var search = value.Trim();
        var hasId = Guid.TryParse(search, out var id);

        var matches = await _readDbContext.Vendors
            .Where(vendor =>
                (hasId && vendor.Id == id) ||
                vendor.Email == search ||
                vendor.Name == search)
            .Select(vendor => new { vendor.Id, vendor.Name, vendor.Email })
            .AsNoTracking()
            .ToListAsync();

        if (matches.Count == 0)
        {
            throw new InvalidOperationException($"Vendor '{search}' was not found.");
        }

        if (matches.Count > 1)
        {
            throw new InvalidOperationException($"Vendor reference '{search}' is ambiguous. Use email or ID.");
        }

        var match = matches[0];
        return (new AgentId(match.Id), match.Name, match.Email);
    }

    private static DateTimeOffset ParseRequiredDateTimeOffset(IReadOnlyDictionary<string, string?> values, string key)
    {
        var value = GetRequiredValue(values, key);

        if (TryParseDateTimeExpression(value, out var result))
        {
            return result;
        }

        throw new InvalidOperationException($"Field '{key}' must be a valid date/time.");
    }

    private static DateTimeOffset? ParseOptionalDateTimeOffset(IReadOnlyDictionary<string, string?> values, string key)
    {
        if (!values.TryGetValue(key, out var value) || string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        if (TryParseDateTimeExpression(value.Trim(), out var result))
        {
            return result;
        }

        throw new InvalidOperationException($"Field '{key}' must be a valid date/time.");
    }

    private static decimal? ParseOptionalDecimal(IReadOnlyDictionary<string, string?> values, string key)
    {
        if (!values.TryGetValue(key, out var value) || string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        if (decimal.TryParse(value.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var result) ||
            decimal.TryParse(value.Trim(), NumberStyles.Number, CultureInfo.CurrentCulture, out result))
        {
            return result;
        }

        throw new InvalidOperationException($"Field '{key}' must be a valid number.");
    }

    private static decimal ParseRequiredDecimal(IReadOnlyDictionary<string, string?> values, string key)
        => ParseOptionalDecimal(values, key)
           ?? throw new InvalidOperationException($"Field '{key}' is required.");

    private static string FormatDateTime(DateTimeOffset value)
        => value.ToString("yyyy-MM-dd HH:mm zzz");

    private static string FormatDateOnly(DateTimeOffset value)
        => value.ToString("yyyy-MM-dd");

    private static string FormatDateOnly(DateTimeOffset? value)
        => value?.ToString("yyyy-MM-dd") ?? "None";

    private static string FormatOptionalDateTime(DateTimeOffset? value)
        => value?.ToString("yyyy-MM-dd HH:mm zzz") ?? "None";

    private static string FormatAmount(decimal? amount)
        => amount?.ToString("0.##", CultureInfo.InvariantCulture) ?? "None";

    private static string FormatNumber(decimal value)
        => value.ToString("0.##", CultureInfo.InvariantCulture);

    private static bool TryParseDateTimeExpression(string value, out DateTimeOffset result)
    {
        var trimmed = value.Trim();
        var now = DateTimeOffset.Now;

        if (string.Equals(trimmed, "N", StringComparison.OrdinalIgnoreCase))
        {
            result = now;
            return true;
        }

        if (string.Equals(trimmed, "T", StringComparison.OrdinalIgnoreCase))
        {
            result = new DateTimeOffset(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, now.Offset);
            return true;
        }

        if (TryParseRelativeDateTimeExpression(trimmed, now, out result))
        {
            return true;
        }

        return DateTimeOffset.TryParse(trimmed, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out result) ||
               DateTimeOffset.TryParse(trimmed, CultureInfo.CurrentCulture, DateTimeStyles.AllowWhiteSpaces, out result);
    }

    private static bool TryParseRelativeDateTimeExpression(string value, DateTimeOffset now, out DateTimeOffset result)
    {
        result = default;

        if (string.IsNullOrWhiteSpace(value) || (value[0] != '+' && value[0] != '-'))
        {
            return false;
        }

        var sign = value[0] == '+' ? 1 : -1;
        var numberPart = value.Substring(1, value.Length - 2);
        var unit = char.ToLowerInvariant(value[^1]);

        if (!double.TryParse(numberPart, NumberStyles.Float, CultureInfo.InvariantCulture, out var amount) &&
            !double.TryParse(numberPart, NumberStyles.Float, CultureInfo.CurrentCulture, out amount))
        {
            return false;
        }

        var signedAmount = amount * sign;
        result = unit switch
        {
            'm' => now.AddMinutes(signedAmount),
            'h' => now.AddHours(signedAmount),
            'd' => now.AddDays(signedAmount),
            'w' => now.AddDays(signedAmount * 7),
            _ => default
        };

        return unit is 'm' or 'h' or 'd' or 'w';
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

            if (targetType == typeof(CommitmentId))
            {
                convertedArgument = new CommitmentId(guid);
                return true;
            }

            if (targetType == typeof(ParticipationId))
            {
                convertedArgument = new ParticipationId(guid);
                return true;
            }

            if (targetType == typeof(StockflowId))
            {
                convertedArgument = new StockflowId(guid);
                return true;
            }

            if (targetType == typeof(StockflowEndId))
            {
                convertedArgument = new StockflowEndId(guid);
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

            if (targetType == typeof(ContractDepartmentCode))
            {
                convertedArgument = new ContractDepartmentCode(value);
                return true;
            }

            if (targetType == typeof(ContractServiceName))
            {
                convertedArgument = new ContractServiceName(value);
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
    Func<EntityWorkspaceBackendService, Guid, Task<EntityWorkspaceOperationResultDto>>? DeleteAsync,
    Func<EntityWorkspaceBackendService, Guid, string, string, IReadOnlyDictionary<string, string?>, Task<EntityWorkspaceOperationResultDto>>? ExecuteCollectionActionAsync = null,
    Func<EntityWorkspaceBackendService, Guid, string, string, string, Task<EntityWorkspaceOperationResultDto>>? ExecuteCollectionItemActionAsync = null);

internal sealed record ItContractReportContractRow(
    Guid Id,
    string ServiceName,
    string DepartmentCode,
    DateTimeOffset When,
    DateTimeOffset? EndWhen,
    decimal? Amount,
    Guid EmployeeId,
    Guid VendorId,
    string EmployeeEmail,
    string VendorEmail);
