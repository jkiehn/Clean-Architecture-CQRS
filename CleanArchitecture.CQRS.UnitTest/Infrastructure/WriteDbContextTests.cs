using CleanArchitectureCQRS.Domain.Factories;
using CleanArchitectureCQRS.Domain.Policies;
using CleanArchitectureCQRS.Domain.ValueObjects;
using CleanArchitectureCQRS.Infrastructure.EF;
using CleanArchitectureCQRS.Infrastructure.EF.Contexts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace CleanArchitecture.CQRS.UnitTest.Infrastructure;

public class WriteDbContextTests
{
    [Fact]
    public async Task Removing_Item_From_Aggregate_Deletes_Child_Row_Instead_Of_Nulling_Foreign_Key()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<WriteDbContext>()
            .UseSqlite(connection)
            .Options;

        var entityId = Guid.NewGuid();

        await using (var arrangeContext = new WriteDbContext(options))
        {
            await arrangeContext.Database.EnsureCreatedAsync();

            var factory = new SampleEntityFactory(Enumerable.Empty<ISampleEntityItemsPolicy>());
            var sampleEntity = factory.Create(
                new SampleEntityId(entityId),
                new SampleEntityName("Test entity"),
                new SampleEntityDestination("Aarhus", "Denmark"));

            sampleEntity.AddItem(new SampleEntityItem("Item 1", 1));
            arrangeContext.SampleEntities.Add(sampleEntity);
            await arrangeContext.SaveChangesAsync();
        }

        await using (var actContext = new WriteDbContext(options))
        {
            var sampleEntity = await actContext.SampleEntities
                .Include("_items")
                .SingleAsync(x => x.Id == new SampleEntityId(entityId));

            sampleEntity.RemoveItem("Item 1");
            actContext.SampleEntities.Update(sampleEntity);

            var exception = await Record.ExceptionAsync(() => actContext.SaveChangesAsync());

            exception.ShouldBeNull();
        }

        await using (var assertContext = new WriteDbContext(options))
        {
            var itemCount = await assertContext.Set<SampleEntityItem>().CountAsync();
            itemCount.ShouldBe(0);
        }
    }

    [Fact]
    public async Task SqlSchemaInitializer_Should_Be_A_NoOp_For_NonSqlServer_Providers()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var services = new ServiceCollection();
        services.AddDbContext<WriteDbContext>(options => options.UseSqlite(connection));

        await using var provider = services.BuildServiceProvider();

        var exception = await Record.ExceptionAsync(() => provider.EnsureAsync());

        exception.ShouldBeNull();
    }
}
