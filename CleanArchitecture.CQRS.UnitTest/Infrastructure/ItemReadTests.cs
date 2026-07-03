using CleanArchitectureCQRS.Application.Queries;
using CleanArchitectureCQRS.Infrastructure.EF.Contexts;
using CleanArchitectureCQRS.Infrastructure.EF.Models;
using CleanArchitectureCQRS.Infrastructure.EF.Queries.Handlers;
using CleanArchitectureCQRS.Infrastructure.EF.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace CleanArchitecture.CQRS.UnitTest.Infrastructure;

public class ItemReadTests
{
    [Fact]
    public async Task ItemReadService_Should_Respect_Excluded_Item_Id()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<ReadDbContext>()
            .UseSqlite(connection)
            .Options;

        var itemId = Guid.NewGuid();

        await using (var arrangeContext = new ReadDbContext(options))
        {
            await arrangeContext.Database.EnsureCreatedAsync();
            arrangeContext.Items.Add(new ItemReadModel
            {
                Id = itemId,
                Name = "Widget"
            });
            await arrangeContext.SaveChangesAsync();
        }

        await using var assertContext = new ReadDbContext(options);
        var service = new ItemReadService(assertContext);

        (await service.ExistsByNameAsync("Widget")).ShouldBeTrue();
        (await service.ExistsByNameAsync("Widget", itemId)).ShouldBeFalse();
    }

    [Fact]
    public async Task SearchItems_Should_Filter_And_Order_Results()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<ReadDbContext>()
            .UseSqlite(connection)
            .Options;

        await using (var arrangeContext = new ReadDbContext(options))
        {
            await arrangeContext.Database.EnsureCreatedAsync();
            arrangeContext.Items.AddRange(
                new ItemReadModel { Id = Guid.NewGuid(), Name = "Charlie Widget" },
                new ItemReadModel { Id = Guid.NewGuid(), Name = "Alpha Widget" },
                new ItemReadModel { Id = Guid.NewGuid(), Name = "Bolt" });
            await arrangeContext.SaveChangesAsync();
        }

        await using var queryContext = new ReadDbContext(options);
        var handler = new SearchItemsHandler(queryContext);

        var result = (await handler.HandleAsync(new SearchItems { SearchPhrase = "Widget" })).ToList();

        result.Count.ShouldBe(2);
        result[0].Name.ShouldBe("Alpha Widget");
        result[1].Name.ShouldBe("Charlie Widget");
    }
}
