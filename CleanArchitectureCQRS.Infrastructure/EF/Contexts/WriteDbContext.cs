using CleanArchitectureCQRS.Domain.Entities;
using CleanArchitectureCQRS.Domain.ValueObjects;
using CleanArchitectureCQRS.Infrastructure.EF.Config;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureCQRS.Infrastructure.EF.Contexts;

internal sealed class WriteDbContext : DbContext
{
    public DbSet<SampleEntity> SampleEntities { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Vendor> Vendors { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Item> Items { get; set; }

    public WriteDbContext(DbContextOptions<WriteDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("SampleEntity");

        var configuration = new WriteConfiguration();
        modelBuilder.ApplyConfiguration<SampleEntity>(configuration);
        modelBuilder.ApplyConfiguration<SampleEntityItem>(configuration);
        modelBuilder.ApplyConfiguration<Customer>(configuration);
        modelBuilder.ApplyConfiguration<Vendor>(configuration);
        modelBuilder.ApplyConfiguration<Employee>(configuration);
        modelBuilder.ApplyConfiguration<Item>(configuration);
    }
}
