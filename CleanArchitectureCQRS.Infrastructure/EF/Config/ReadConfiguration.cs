using CleanArchitectureCQRS.Infrastructure.EF.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitectureCQRS.Infrastructure.EF.Config;

internal sealed class ReadConfiguration : IEntityTypeConfiguration<SampleEntityReadModel>, IEntityTypeConfiguration<SampleEntityItemReadModel>, IEntityTypeConfiguration<CustomerReadModel>, IEntityTypeConfiguration<VendorReadModel>
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

    private static void ConfigureAgent<TAgent>(EntityTypeBuilder<TAgent> builder, string tableName) where TAgent : AgentReadModelBase
    {
        builder.ToTable(tableName);
        builder.HasKey(agent => agent.Id);
        builder.Property(agent => agent.Name).IsRequired();
        builder.Property(agent => agent.Email).IsRequired();
    }
}
