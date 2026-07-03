using CleanArchitectureCQRS.Infrastructure.EF.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitectureCQRS.Infrastructure.EF.Config;

internal sealed class ReadConfiguration : IEntityTypeConfiguration<SampleEntityReadModel>, IEntityTypeConfiguration<SampleEntityItemReadModel>, IEntityTypeConfiguration<CustomerReadModel>, IEntityTypeConfiguration<VendorReadModel>, IEntityTypeConfiguration<EmployeeReadModel>, IEntityTypeConfiguration<ItemReadModel>, IEntityTypeConfiguration<SaleReadModel>, IEntityTypeConfiguration<SalesLineReadModel>
{
    public void Configure(EntityTypeBuilder<SampleEntityReadModel> builder)
    {
        builder.ToTable("SampleEntity");
        builder.HasKey(pl => pl.Id);

        builder
            .Property(pl => pl.Destination)
            .HasConversion(l => l.ToString(), l => DestinationReadModel.Create(l));

        builder
            .HasMany(pl => pl.Items)
            .WithOne(pi => pi.SampleEntity);
    }

    public void Configure(EntityTypeBuilder<SampleEntityItemReadModel> builder)
    {
        builder.ToTable("SampleEntityItems");
    }

    public void Configure(EntityTypeBuilder<CustomerReadModel> builder)
        => ConfigureAgent(builder, "Customers");

    public void Configure(EntityTypeBuilder<VendorReadModel> builder)
        => ConfigureAgent(builder, "Vendors");

    public void Configure(EntityTypeBuilder<EmployeeReadModel> builder)
    {
        ConfigureAgent(builder, "Employees");
        builder.Property(employee => employee.SocialSecurityNumber).IsRequired();
    }

    public void Configure(EntityTypeBuilder<ItemReadModel> builder)
        => ConfigureResource(builder, "Items");

    public void Configure(EntityTypeBuilder<SaleReadModel> builder)
    {
        ConfigureEvent(builder, "Sales");
        builder.Property(sale => sale.InternalParticipationId).IsRequired();
        builder.Property(sale => sale.EmployeeId).IsRequired();
        builder.Property(sale => sale.ExternalParticipationId).IsRequired();
        builder.Property(sale => sale.CustomerId).IsRequired();
    }

    public void Configure(EntityTypeBuilder<SalesLineReadModel> builder)
    {
        builder.ToTable("SalesLines");
        builder.HasKey(line => line.Id);
        builder.Property(line => line.EventEndId).IsRequired();
        builder.Property(line => line.ResourceEndId).IsRequired();
        builder.Property(line => line.SaleId).IsRequired();
        builder.Property(line => line.ItemId).IsRequired();
        builder.Property(line => line.UnitPrice).IsRequired();
        builder.Property(line => line.Quantity).IsRequired();
    }

    private static void ConfigureAgent<TAgent>(EntityTypeBuilder<TAgent> builder, string tableName) where TAgent : AgentReadModelBase
    {
        builder.ToTable(tableName);
        builder.HasKey(agent => agent.Id);
        builder.Property(agent => agent.Name).IsRequired();
        builder.Property(agent => agent.Email).IsRequired();
    }

    private static void ConfigureResource<TResource>(EntityTypeBuilder<TResource> builder, string tableName) where TResource : ResourceReadModelBase
    {
        builder.ToTable(tableName);
        builder.HasKey(resource => resource.Id);
        builder.Property(resource => resource.Name).IsRequired();
    }

    private static void ConfigureEvent<TEvent>(EntityTypeBuilder<TEvent> builder, string tableName) where TEvent : EventReadModelBase
    {
        builder.ToTable(tableName);
        builder.HasKey(@event => @event.Id);
        builder.Property(@event => @event.When).IsRequired();
        builder.Property(@event => @event.EndWhen);
        builder.Property(@event => @event.Amount);
    }

    private static void ConfigureParticipation<TParticipation>(EntityTypeBuilder<TParticipation> builder, string tableName) where TParticipation : ParticipationReadModelBase
    {
        builder.ToTable(tableName);
        builder.HasKey(participation => participation.Id);
        builder.Property(participation => participation.AgentId).IsRequired();
        builder.Property(participation => participation.EventId).IsRequired();
    }
}
