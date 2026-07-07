using CleanArchitectureCQRS.Infrastructure.EF.Config;
using CleanArchitectureCQRS.Infrastructure.EF.Models;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureCQRS.Infrastructure.EF.Contexts;

internal sealed class ReadDbContext : DbContext
{
    public DbSet<SampleEntityReadModel> SampleEntities { get; set; }
    public DbSet<CustomerReadModel> Customers { get; set; }
    public DbSet<VendorReadModel> Vendors { get; set; }
    public DbSet<EmployeeReadModel> Employees { get; set; }
    public DbSet<ItemReadModel> Items { get; set; }
    public DbSet<SaleReadModel> Sales { get; set; }
    public DbSet<SalesLineReadModel> SalesLines { get; set; }
    public DbSet<SalesOrderReadModel> SalesOrders { get; set; }
    public DbSet<SalesOrderLineReadModel> SalesOrderLines { get; set; }
    public DbSet<ItContractReadModel> ItContracts { get; set; }

    public ReadDbContext(DbContextOptions<ReadDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("SampleEntity");

        var configuration = new ReadConfiguration();
        modelBuilder.ApplyConfiguration<SampleEntityReadModel>(configuration);
        modelBuilder.ApplyConfiguration<SampleEntityItemReadModel>(configuration);
        modelBuilder.ApplyConfiguration<CustomerReadModel>(configuration);
        modelBuilder.ApplyConfiguration<VendorReadModel>(configuration);
        modelBuilder.ApplyConfiguration<EmployeeReadModel>(configuration);
        modelBuilder.ApplyConfiguration<ItemReadModel>(configuration);
        modelBuilder.ApplyConfiguration<SaleReadModel>(configuration);
        modelBuilder.ApplyConfiguration<SalesLineReadModel>(configuration);
        modelBuilder.ApplyConfiguration<SalesOrderReadModel>(configuration);
        modelBuilder.ApplyConfiguration<SalesOrderLineReadModel>(configuration);
        modelBuilder.ApplyConfiguration<ItContractReadModel>(configuration);
    }
}
