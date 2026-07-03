using CleanArchitectureCQRS.Domain.Entities;
using CleanArchitectureCQRS.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CleanArchitectureCQRS.Infrastructure.EF.Config;

internal sealed class WriteConfiguration : IEntityTypeConfiguration<SampleEntity>, IEntityTypeConfiguration<SampleEntityItem>, IEntityTypeConfiguration<Customer>, IEntityTypeConfiguration<Vendor>, IEntityTypeConfiguration<Employee>, IEntityTypeConfiguration<Item>, IEntityTypeConfiguration<Sale>, IEntityTypeConfiguration<SalesLine>
{
    public void Configure(EntityTypeBuilder<SampleEntity> builder)
    {
        builder.HasKey(pl => pl.Id);

        var destinationConverter = new ValueConverter<SampleEntityDestination, string>(l => l.ToString(),
            l => SampleEntityDestination.Create(l));

        var packingListNameConverter = new ValueConverter<SampleEntityName, string>(pln => pln.Value,
            pln => new SampleEntityName(pln));

        builder
            .Property(pl => pl.Id)
            .HasConversion(id => id.Value, id => new SampleEntityId(id));

        builder
            .Property(typeof(SampleEntityDestination), "_destination")
            .HasConversion(destinationConverter)
            .HasColumnName("Destination");

        builder
            .Property(typeof(SampleEntityName), "_name")
            .HasConversion(packingListNameConverter)
            .HasColumnName("Name");

        builder
            .HasMany(typeof(SampleEntityItem), "_items")
            .WithOne()
            .HasForeignKey("SampleEntityId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable("SampleEntity");
    }

    public void Configure(EntityTypeBuilder<SampleEntityItem> builder)
    {
        builder.Property<Guid>("Id");
        builder
            .Property<SampleEntityId>("SampleEntityId")
            .HasConversion(id => id.Value, id => new SampleEntityId(id));
        builder.Property(pi => pi.Name);
        builder.Property(pi => pi.Quantity);
        builder.Property(pi => pi.IsTaken);
        builder.ToTable("SampleEntityItems");
    }

    public void Configure(EntityTypeBuilder<Customer> builder)
        => ConfigureAgent(builder, "Customers");

    public void Configure(EntityTypeBuilder<Vendor> builder)
        => ConfigureAgent(builder, "Vendors");

    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        ConfigureAgent(builder, "Employees");

        builder
            .Property<string>("_socialSecurityNumber")
            .HasColumnName("SocialSecurityNumber")
            .IsRequired();
    }

    public void Configure(EntityTypeBuilder<Item> builder)
        => ConfigureResource(builder, "Items");

    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        ConfigureEvent(builder, "Sales");

        builder
            .Property<ParticipationId>("_internalParticipationId")
            .HasConversion(id => id.Value, id => new ParticipationId(id))
            .HasColumnName("InternalParticipationId")
            .IsRequired();

        builder
            .Property<AgentId>("_employeeId")
            .HasConversion(id => id.Value, id => new AgentId(id))
            .HasColumnName("EmployeeId")
            .IsRequired();

        builder
            .Property<ParticipationId>("_externalParticipationId")
            .HasConversion(id => id.Value, id => new ParticipationId(id))
            .HasColumnName("ExternalParticipationId")
            .IsRequired();

        builder
            .Property<AgentId>("_customerId")
            .HasConversion(id => id.Value, id => new AgentId(id))
            .HasColumnName("CustomerId")
            .IsRequired();

        builder
            .HasMany(typeof(SalesLine), "_salesLines")
            .WithOne()
            .HasForeignKey("_saleId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }

    public void Configure(EntityTypeBuilder<SalesLine> builder)
    {
        ConfigureStockflow(builder, "SalesLines");

        builder
            .Property<EventId>("_saleId")
            .HasConversion(id => id.Value, id => new EventId(id))
            .HasColumnName("SaleId")
            .IsRequired();

        builder
            .Property<ResourceId>("_itemId")
            .HasConversion(id => id.Value, id => new ResourceId(id))
            .HasColumnName("ItemId")
            .IsRequired();

        builder
            .Property<decimal>("_unitPrice")
            .HasColumnName("UnitPrice")
            .IsRequired();

        builder
            .Property<decimal>("_quantity")
            .HasColumnName("Quantity")
            .IsRequired();
    }

    private static void ConfigureAgent<TAgent>(EntityTypeBuilder<TAgent> builder, string tableName) where TAgent : Agent
    {
        var agentNameConverter = new ValueConverter<AgentName, string>(name => name.Value,
            value => new AgentName(value));
        var agentEmailConverter = new ValueConverter<AgentEmail, string>(email => email.Value,
            value => new AgentEmail(value));

        builder.HasKey(customer => customer.Id);

        builder
            .Property(customer => customer.Id)
            .HasConversion(id => id.Value, id => new AgentId(id));

        builder
            .Property(typeof(AgentName), "_name")
            .HasConversion(agentNameConverter)
            .HasColumnName("Name")
            .IsRequired();

        builder
            .Property(typeof(AgentEmail), "_email")
            .HasConversion(agentEmailConverter)
            .HasColumnName("Email")
            .IsRequired();

        builder.ToTable(tableName);
    }

    private static void ConfigureResource<TResource>(EntityTypeBuilder<TResource> builder, string tableName) where TResource : Resource
    {
        var resourceNameConverter = new ValueConverter<ResourceName, string>(name => name.Value,
            value => new ResourceName(value));

        builder.HasKey(resource => resource.Id);

        builder
            .Property(resource => resource.Id)
            .HasConversion(id => id.Value, id => new ResourceId(id));

        builder
            .Property(typeof(ResourceName), "_name")
            .HasConversion(resourceNameConverter)
            .HasColumnName("Name")
            .IsRequired();

        builder.ToTable(tableName);
    }

    private static void ConfigureEvent<TEvent>(EntityTypeBuilder<TEvent> builder, string tableName) where TEvent : Domain.Entities.Event
    {
        builder.HasKey(@event => @event.Id);

        builder
            .Property(@event => @event.Id)
            .HasConversion(id => id.Value, id => new EventId(id));

        builder
            .Property<DateTimeOffset>("_when")
            .HasColumnName("When")
            .IsRequired();

        builder
            .Property<DateTimeOffset?>("_endWhen")
            .HasColumnName("EndWhen");

        builder
            .Property<decimal?>("_amount")
            .HasColumnName("Amount");

        builder.ToTable(tableName);
    }

    private static void ConfigureParticipation<TParticipation>(EntityTypeBuilder<TParticipation> builder, string tableName) where TParticipation : Participation
    {
        builder.HasKey(participation => participation.Id);

        builder
            .Property(participation => participation.Id)
            .HasConversion(id => id.Value, id => new ParticipationId(id));

        builder
            .Property<AgentId>("_agentId")
            .HasConversion(id => id.Value, id => new AgentId(id))
            .HasColumnName("AgentId")
            .IsRequired();

        builder
            .Property<EventId>("_eventId")
            .HasConversion(id => id.Value, id => new EventId(id))
            .HasColumnName("EventId")
            .IsRequired();

        builder.ToTable(tableName);
    }

    private static void ConfigureStockflow<TStockflow>(EntityTypeBuilder<TStockflow> builder, string tableName) where TStockflow : Stockflow
    {
        builder.HasKey(stockflow => stockflow.Id);

        builder
            .Property(stockflow => stockflow.Id)
            .HasConversion(id => id.Value, id => new StockflowId(id));

        builder
            .Property<StockflowEndId>("_eventEndId")
            .HasConversion(id => id.Value, id => new StockflowEndId(id))
            .HasColumnName("EventEndId")
            .IsRequired();

        builder
            .Property<StockflowEndId>("_resourceEndId")
            .HasConversion(id => id.Value, id => new StockflowEndId(id))
            .HasColumnName("ResourceEndId")
            .IsRequired();

        builder.ToTable(tableName);
    }
}
