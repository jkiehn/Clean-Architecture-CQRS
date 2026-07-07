using CleanArchitectureCQRS.Application.Services;
using CleanArchitectureCQRS.Domain.Entities;
using CleanArchitectureCQRS.Infrastructure;
using CleanArchitectureCQRS.Infrastructure.EF.Contexts;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace CleanArchitecture.CQRS.UnitTest.Infrastructure;

public class CustomerPaymentWorkspaceTests
{
    [Fact]
    public async Task CreateAsync_Creates_Cash_Through_Generic_Backend_Service()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var writeOptions = new DbContextOptionsBuilder<WriteDbContext>()
            .UseSqlite(connection)
            .Options;
        var readOptions = new DbContextOptionsBuilder<ReadDbContext>()
            .UseSqlite(connection)
            .Options;

        await using (var arrangeContext = new WriteDbContext(writeOptions))
        {
            await arrangeContext.Database.EnsureCreatedAsync();
        }

        await using var readContext = new ReadDbContext(readOptions);
        await using var writeContext = new WriteDbContext(writeOptions);
        IEntityWorkspaceBackendService service = new EntityWorkspaceBackendService(readContext, writeContext);

        var result = await service.CreateAsync("cash", new Dictionary<string, string?>
        {
            ["name"] = "Main Register"
        });

        result.EntityId.ShouldNotBeNull();
        result.Message.ShouldBe("Cash created.");

        await using var assertReadContext = new ReadDbContext(readOptions);
        var storedCash = await assertReadContext.CashResources.SingleAsync();
        storedCash.Name.ShouldBe("Main Register");
    }

    [Fact]
    public async Task CreateAsync_Creates_CustomerPayment_With_Customer_And_Cash()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var writeOptions = new DbContextOptionsBuilder<WriteDbContext>()
            .UseSqlite(connection)
            .Options;
        var readOptions = new DbContextOptionsBuilder<ReadDbContext>()
            .UseSqlite(connection)
            .Options;

        var customerId = Guid.NewGuid();
        var cashId = Guid.NewGuid();

        await using (var arrangeContext = new WriteDbContext(writeOptions))
        {
            await arrangeContext.Database.EnsureCreatedAsync();
            arrangeContext.Customers.Add(new Customer(customerId, "Contoso", "buyer@contoso.example"));
            arrangeContext.CashResources.Add(new Cash(cashId, "Main Register"));
            await arrangeContext.SaveChangesAsync();
        }

        await using var readContext = new ReadDbContext(readOptions);
        await using var writeContext = new WriteDbContext(writeOptions);
        IEntityWorkspaceBackendService service = new EntityWorkspaceBackendService(readContext, writeContext);

        var result = await service.CreateAsync("customer-payments", new Dictionary<string, string?>
        {
            ["when"] = "2026-07-07T09:30:00+00:00",
            ["amount"] = "125.50",
            ["customer"] = "buyer@contoso.example",
            ["cash"] = cashId.ToString()
        });

        result.EntityId.ShouldNotBeNull();
        result.Message.ShouldBe("Customer payment created.");

        await using var assertReadContext = new ReadDbContext(readOptions);
        var storedPayment = await assertReadContext.CustomerPayments.SingleAsync();
        storedPayment.CustomerId.ShouldBe(customerId);
        storedPayment.CashResourceId.ShouldBe(cashId);
        storedPayment.Amount.ShouldBe(125.50m);
    }

    [Fact]
    public async Task ExecuteCollectionActionAsync_Links_CustomerPayment_To_Sale()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var writeOptions = new DbContextOptionsBuilder<WriteDbContext>()
            .UseSqlite(connection)
            .Options;
        var readOptions = new DbContextOptionsBuilder<ReadDbContext>()
            .UseSqlite(connection)
            .Options;

        var customerId = Guid.NewGuid();
        var employeeId = Guid.NewGuid();
        var saleId = Guid.NewGuid();
        var paymentId = Guid.NewGuid();
        var cashId = Guid.NewGuid();

        await using (var arrangeContext = new WriteDbContext(writeOptions))
        {
            await arrangeContext.Database.EnsureCreatedAsync();
            arrangeContext.Employees.Add(new Employee(employeeId, "Emma", "emma@example.com", "444-44-4444"));
            arrangeContext.Customers.Add(new Customer(customerId, "Contoso", "buyer@contoso.example"));
            arrangeContext.CashResources.Add(new Cash(cashId, "Main Register"));
            arrangeContext.Sales.Add(new Sale(saleId, new DateTimeOffset(2026, 7, 7, 10, 0, 0, TimeSpan.Zero), employeeId, customerId, amount: 125.50m));
            arrangeContext.CustomerPayments.Add(new CustomerPayment(paymentId, new DateTimeOffset(2026, 7, 7, 10, 0, 0, TimeSpan.Zero), customerId, cashId, 125.50m));
            await arrangeContext.SaveChangesAsync();
        }

        await using var readContext = new ReadDbContext(readOptions);
        await using var writeContext = new WriteDbContext(writeOptions);
        IEntityWorkspaceBackendService service = new EntityWorkspaceBackendService(readContext, writeContext);

        var result = await service.ExecuteCollectionActionAsync("sales", saleId, "payments", "link", new Dictionary<string, string?>
        {
            ["customerPayment"] = paymentId.ToString()
        });

        result.Message.ShouldBe("Customer payment linked to sale.");

        await using var assertReadContext = new ReadDbContext(readOptions);
        var storedLink = await assertReadContext.PaysFors.SingleAsync();
        storedLink.SaleId.ShouldBe(saleId);
        storedLink.CustomerPaymentId.ShouldBe(paymentId);
    }

    [Fact]
    public async Task ExecuteCollectionActionAsync_Creates_And_Links_CustomerPayment_For_Sale()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var writeOptions = new DbContextOptionsBuilder<WriteDbContext>()
            .UseSqlite(connection)
            .Options;
        var readOptions = new DbContextOptionsBuilder<ReadDbContext>()
            .UseSqlite(connection)
            .Options;

        var customerId = Guid.NewGuid();
        var employeeId = Guid.NewGuid();
        var saleId = Guid.NewGuid();
        var cashId = Guid.NewGuid();

        await using (var arrangeContext = new WriteDbContext(writeOptions))
        {
            await arrangeContext.Database.EnsureCreatedAsync();
            arrangeContext.Employees.Add(new Employee(employeeId, "Emma", "emma@example.com", "444-44-4444"));
            arrangeContext.Customers.Add(new Customer(customerId, "Contoso", "buyer@contoso.example"));
            arrangeContext.CashResources.Add(new Cash(cashId, "Main Register"));
            arrangeContext.Sales.Add(new Sale(saleId, new DateTimeOffset(2026, 7, 7, 10, 0, 0, TimeSpan.Zero), employeeId, customerId, amount: 125.50m));
            await arrangeContext.SaveChangesAsync();
        }

        await using var readContext = new ReadDbContext(readOptions);
        await using var writeContext = new WriteDbContext(writeOptions);
        IEntityWorkspaceBackendService service = new EntityWorkspaceBackendService(readContext, writeContext);

        var result = await service.ExecuteCollectionActionAsync("sales", saleId, "payments", "add", new Dictionary<string, string?>
        {
            ["when"] = "N",
            ["cash"] = cashId.ToString()
        });

        result.Message.ShouldBe("Customer payment created and linked to sale.");

        await using var assertReadContext = new ReadDbContext(readOptions);
        var storedPayment = await assertReadContext.CustomerPayments.SingleAsync();
        var storedLink = await assertReadContext.PaysFors.SingleAsync();

        storedPayment.CustomerId.ShouldBe(customerId);
        storedPayment.CashResourceId.ShouldBe(cashId);
        storedPayment.Amount.ShouldBe(125.50m);
        storedLink.SaleId.ShouldBe(saleId);
        storedLink.CustomerPaymentId.ShouldBe(storedPayment.Id);
    }

    [Fact]
    public async Task CreateAsync_Creates_PaysFor_Through_Backend_Service()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var writeOptions = new DbContextOptionsBuilder<WriteDbContext>()
            .UseSqlite(connection)
            .Options;
        var readOptions = new DbContextOptionsBuilder<ReadDbContext>()
            .UseSqlite(connection)
            .Options;

        var customerId = Guid.NewGuid();
        var employeeId = Guid.NewGuid();
        var saleId = Guid.NewGuid();
        var paymentId = Guid.NewGuid();
        var cashId = Guid.NewGuid();

        await using (var arrangeContext = new WriteDbContext(writeOptions))
        {
            await arrangeContext.Database.EnsureCreatedAsync();
            arrangeContext.Employees.Add(new Employee(employeeId, "Emma", "emma@example.com", "444-44-4444"));
            arrangeContext.Customers.Add(new Customer(customerId, "Contoso", "buyer@contoso.example"));
            arrangeContext.CashResources.Add(new Cash(cashId, "Main Register"));
            arrangeContext.Sales.Add(new Sale(saleId, new DateTimeOffset(2026, 7, 7, 10, 0, 0, TimeSpan.Zero), employeeId, customerId, amount: 125.50m));
            arrangeContext.CustomerPayments.Add(new CustomerPayment(paymentId, new DateTimeOffset(2026, 7, 7, 10, 5, 0, TimeSpan.Zero), customerId, cashId, 125.50m));
            await arrangeContext.SaveChangesAsync();
        }

        await using var readContext = new ReadDbContext(readOptions);
        await using var writeContext = new WriteDbContext(writeOptions);
        IEntityWorkspaceBackendService service = new EntityWorkspaceBackendService(readContext, writeContext);

        var result = await service.CreateAsync("pays-for", new Dictionary<string, string?>
        {
            ["sale"] = saleId.ToString(),
            ["customerPayment"] = paymentId.ToString()
        });

        result.Message.ShouldBe("Customer payment linked to sale.");

        await using var assertReadContext = new ReadDbContext(readOptions);
        var storedLink = await assertReadContext.PaysFors.SingleAsync();
        storedLink.SaleId.ShouldBe(saleId);
        storedLink.CustomerPaymentId.ShouldBe(paymentId);
    }
}
