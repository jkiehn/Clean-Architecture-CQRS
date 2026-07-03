using CleanArchitectureCQRS.Application.Services;
using CleanArchitectureCQRS.Domain.Entities;
using CleanArchitectureCQRS.Infrastructure;
using CleanArchitectureCQRS.Infrastructure.EF.Contexts;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace CleanArchitecture.CQRS.UnitTest.Infrastructure;

public class EntityWorkspaceBackendServiceTests
{
    [Fact]
    public async Task SearchAsync_Returns_Generic_Customer_Workspace_Items()
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
            arrangeContext.Customers.AddRange(
                new Customer(Guid.NewGuid(), "Charlie", "charlie@example.com"),
                new Customer(Guid.NewGuid(), "Alice", "alice@example.com"));
            await arrangeContext.SaveChangesAsync();
        }

        await using var readContext = new ReadDbContext(readOptions);
        await using var writeContext = new WriteDbContext(writeOptions);
        IEntityWorkspaceBackendService service = new EntityWorkspaceBackendService(readContext, writeContext);

        var result = await service.SearchAsync("customers", "example.com");

        result.Count.ShouldBe(2);
        result[0].Title.ShouldBe("Alice");
        result[0].Meta.Single().Value.ShouldBe("Customer");
        result[1].Title.ShouldBe("Charlie");
    }

    [Fact]
    public async Task CreateAsync_Creates_Item_Through_Generic_Backend_Service()
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

        var result = await service.CreateAsync("items", new Dictionary<string, string?>
        {
            ["name"] = "Widget"
        });

        result.EntityId.ShouldNotBeNull();
        result.Message.ShouldBe("Item created.");

        await using var assertReadContext = new ReadDbContext(readOptions);
        var storedItem = await assertReadContext.Items.SingleAsync();
        storedItem.Name.ShouldBe("Widget");
    }

    [Fact]
    public async Task DeleteAsync_Rejects_Read_Only_Agent_Abstraction()
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

        var exception = await Record.ExceptionAsync(() => service.DeleteAsync("agents", Guid.NewGuid()));

        exception.ShouldBeOfType<InvalidOperationException>();
        exception.Message.ShouldContain("does not support deletion");
    }
}
