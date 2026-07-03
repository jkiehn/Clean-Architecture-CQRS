using CleanArchitectureCQRS.Infrastructure.EF.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitectureCQRS.Infrastructure.EF.Config;

internal sealed class ReadConfiguration : IEntityTypeConfiguration<SampleEntityReadModel>, IEntityTypeConfiguration<SampleEntityItemReadModel>, IEntityTypeConfiguration<CustomerReadModel>, IEntityTypeConfiguration<VendorReadModel>, IEntityTypeConfiguration<EmployeeReadModel>, IEntityTypeConfiguration<ItemReadModel>
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
}
