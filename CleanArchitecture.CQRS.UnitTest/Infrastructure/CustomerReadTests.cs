using CleanArchitectureCQRS.Application.Queries;
using CleanArchitectureCQRS.Infrastructure.EF.Contexts;
using CleanArchitectureCQRS.Infrastructure.EF.Queries.Handlers;
using CleanArchitectureCQRS.Infrastructure.EF.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace CleanArchitecture.CQRS.UnitTest.Infrastructure;

public class CustomerReadTests
{
    [Fact]
    public async Task CustomerReadService_Should_Respect_Excluded_Customer_Id()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<ReadDbContext>()
            .UseSqlite(connection)
            .Options;

        var customerId = Guid.NewGuid();

        await using (var arrangeContext = new ReadDbContext(options))
        {
            await arrangeContext.Database.EnsureCreatedAsync();
            arrangeContext.Customers.Add(new()
            {
                Id = customerId,
                Name = "Alice",
                Email = "alice@example.com"
            });
            await arrangeContext.SaveChangesAsync();
        }

        await using var assertContext = new ReadDbContext(options);
        var service = new CustomerReadService(assertContext);

        (await service.ExistsByEmailAsync("alice@example.com")).ShouldBeTrue();
        (await service.ExistsByEmailAsync("alice@example.com", customerId)).ShouldBeFalse();
    }

    [Fact]
    public async Task SearchCustomers_Should_Filter_And_Order_Results()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<ReadDbContext>()
            .UseSqlite(connection)
            .Options;

        await using (var arrangeContext = new ReadDbContext(options))
        {
            await arrangeContext.Database.EnsureCreatedAsync();
            arrangeContext.Customers.AddRange(
                new() { Id = Guid.NewGuid(), Name = "Charlie", Email = "charlie@example.com" },
                new() { Id = Guid.NewGuid(), Name = "Alice", Email = "alice@example.com" },
                new() { Id = Guid.NewGuid(), Name = "Bob", Email = "bob@sample.com" });
            await arrangeContext.SaveChangesAsync();
        }

        await using var queryContext = new ReadDbContext(options);
        var handler = new SearchCustomersHandler(queryContext);

        var result = (await handler.HandleAsync(new SearchCustomers { SearchPhrase = "example.com" })).ToList();

        result.Count.ShouldBe(2);
        result[0].Name.ShouldBe("Alice");
        result[1].Name.ShouldBe("Charlie");
    }

    [Fact]
    public async Task SearchAgents_Should_Return_Read_Only_Agent_List()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<ReadDbContext>()
            .UseSqlite(connection)
            .Options;

        await using (var arrangeContext = new ReadDbContext(options))
        {
            await arrangeContext.Database.EnsureCreatedAsync();
            arrangeContext.Customers.AddRange(
                new() { Id = Guid.NewGuid(), Name = "Charlie", Email = "charlie@example.com" },
                new() { Id = Guid.NewGuid(), Name = "Alice", Email = "alice@example.com" });
            await arrangeContext.SaveChangesAsync();
        }

        await using var queryContext = new ReadDbContext(options);
        var handler = new SearchAgentsHandler(queryContext);

        var result = (await handler.HandleAsync(new SearchAgents())).ToList();

        result.Count.ShouldBe(2);
        result[0].Name.ShouldBe("Alice");
        result[0].AgentType.ShouldBe("Customer");
        result[1].Name.ShouldBe("Charlie");
    }
}
