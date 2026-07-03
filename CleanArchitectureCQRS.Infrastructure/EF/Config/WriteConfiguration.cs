using CleanArchitectureCQRS.Domain.Entities;
using CleanArchitectureCQRS.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CleanArchitectureCQRS.Infrastructure.EF.Config;

internal sealed class WriteConfiguration : IEntityTypeConfiguration<SampleEntity>, IEntityTypeConfiguration<SampleEntityItem>, IEntityTypeConfiguration<Customer>, IEntityTypeConfiguration<Vendor>, IEntityTypeConfiguration<Employee>, IEntityTypeConfiguration<Item>
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
}
