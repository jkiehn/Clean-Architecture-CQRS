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

    [Fact]
    public async Task SearchAsync_Returns_Generic_Employee_Workspace_Items()
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
            arrangeContext.Employees.AddRange(
                new Employee(Guid.NewGuid(), "Erin", "erin@example.com", "222-22-2222"),
                new Employee(Guid.NewGuid(), "Adam", "adam@example.com", "111-11-1111"));
            await arrangeContext.SaveChangesAsync();
        }

        await using var readContext = new ReadDbContext(readOptions);
        await using var writeContext = new WriteDbContext(writeOptions);
        IEntityWorkspaceBackendService service = new EntityWorkspaceBackendService(readContext, writeContext);

        var result = await service.SearchAsync("employees", "example.com");

        result.Count.ShouldBe(2);
        var adam = result.Single(item => item.Title == "Adam");
        var erin = result.Single(item => item.Title == "Erin");

        adam.Meta.ShouldContain(property => property.Label == "Type" && property.Value == "Employee");
        adam.Meta.ShouldContain(property => property.Label == "SSN" && property.Value == "111-11-1111");
        erin.Meta.ShouldContain(property => property.Label == "SSN" && property.Value == "222-22-2222");
    }

    [Fact]
    public async Task CreateAsync_Creates_Employee_With_SocialSecurityNumber()
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

        var result = await service.CreateAsync("employees", new Dictionary<string, string?>
        {
            ["name"] = "Eve",
            ["email"] = "eve@example.com",
            ["socialSecurityNumber"] = "333-33-3333"
        });

        result.EntityId.ShouldNotBeNull();
        result.Message.ShouldBe("Employee created.");

        await using var assertReadContext = new ReadDbContext(readOptions);
        var storedEmployee = await assertReadContext.Employees.SingleAsync();
        storedEmployee.Name.ShouldBe("Eve");
        storedEmployee.Email.ShouldBe("eve@example.com");
        storedEmployee.SocialSecurityNumber.ShouldBe("333-33-3333");
    }

    [Fact]
    public async Task GetAsync_Returns_Employee_SocialSecurityNumber()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var writeOptions = new DbContextOptionsBuilder<WriteDbContext>()
            .UseSqlite(connection)
            .Options;
        var readOptions = new DbContextOptionsBuilder<ReadDbContext>()
            .UseSqlite(connection)
            .Options;

        var employeeId = Guid.NewGuid();

        await using (var arrangeContext = new WriteDbContext(writeOptions))
        {
            await arrangeContext.Database.EnsureCreatedAsync();
            arrangeContext.Employees.Add(new Employee(employeeId, "Emma", "emma@example.com", "444-44-4444"));
            await arrangeContext.SaveChangesAsync();
        }

        await using var readContext = new ReadDbContext(readOptions);
        await using var writeContext = new WriteDbContext(writeOptions);
        IEntityWorkspaceBackendService service = new EntityWorkspaceBackendService(readContext, writeContext);

        var detail = await service.GetAsync("employees", employeeId);

        detail.ShouldNotBeNull();
        detail!.Summary.ShouldContain(property => property.Label == "Social Security Number" && property.Value == "444-44-4444");
        detail.EditValues["socialSecurityNumber"].ShouldBe("444-44-4444");
    }

    [Fact]
    public async Task CreateAsync_Creates_Sale_With_Employee_And_Customer_Participations()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var writeOptions = new DbContextOptionsBuilder<WriteDbContext>()
            .UseSqlite(connection)
            .Options;
        var readOptions = new DbContextOptionsBuilder<ReadDbContext>()
            .UseSqlite(connection)
            .Options;

        var employeeId = Guid.NewGuid();
        var customerId = Guid.NewGuid();

        await using (var arrangeContext = new WriteDbContext(writeOptions))
        {
            await arrangeContext.Database.EnsureCreatedAsync();
            arrangeContext.Employees.Add(new Employee(employeeId, "Emma", "emma@example.com", "444-44-4444"));
            arrangeContext.Customers.Add(new Customer(customerId, "Contoso", "buyer@contoso.example"));
            await arrangeContext.SaveChangesAsync();
        }

        await using var readContext = new ReadDbContext(readOptions);
        await using var writeContext = new WriteDbContext(writeOptions);
        IEntityWorkspaceBackendService service = new EntityWorkspaceBackendService(readContext, writeContext);

        var result = await service.CreateAsync("sales", new Dictionary<string, string?>
        {
            ["when"] = "2026-07-03T10:00:00+00:00",
            ["employee"] = "emma@example.com",
            ["customer"] = "buyer@contoso.example"
        });

        result.EntityId.ShouldNotBeNull();
        result.Message.ShouldBe("Sale created.");

        await using var assertReadContext = new ReadDbContext(readOptions);
        var storedSale = await assertReadContext.Sales.SingleAsync();
        storedSale.EmployeeId.ShouldBe(employeeId);
        storedSale.CustomerId.ShouldBe(customerId);
        storedSale.Amount.ShouldBeNull();
    }

    [Fact]
    public async Task GetAsync_Returns_Sale_Detail_With_Employee_And_Customer()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var writeOptions = new DbContextOptionsBuilder<WriteDbContext>()
            .UseSqlite(connection)
            .Options;
        var readOptions = new DbContextOptionsBuilder<ReadDbContext>()
            .UseSqlite(connection)
            .Options;

        var saleId = Guid.NewGuid();
        var employeeId = Guid.NewGuid();
        var customerId = Guid.NewGuid();

        await using (var arrangeContext = new WriteDbContext(writeOptions))
        {
            await arrangeContext.Database.EnsureCreatedAsync();
            arrangeContext.Employees.Add(new Employee(employeeId, "Emma", "emma@example.com", "444-44-4444"));
            arrangeContext.Customers.Add(new Customer(customerId, "Contoso", "buyer@contoso.example"));
            arrangeContext.Items.Add(new Item(Guid.NewGuid(), "Widget"));
            arrangeContext.Sales.Add(new Sale(saleId, new DateTimeOffset(2026, 7, 3, 10, 0, 0, TimeSpan.Zero), employeeId, customerId, amount: 125.50m));
            await arrangeContext.SaveChangesAsync();
        }

        await using var readContext = new ReadDbContext(readOptions);
        await using var writeContext = new WriteDbContext(writeOptions);
        IEntityWorkspaceBackendService service = new EntityWorkspaceBackendService(readContext, writeContext);

        var detail = await service.GetAsync("sales", saleId);

        detail.ShouldNotBeNull();
        detail!.Summary.ShouldContain(property => property.Label == "Employee" && property.Value.Contains("emma@example.com"));
        detail.Summary.ShouldContain(property => property.Label == "Customer" && property.Value.Contains("buyer@contoso.example"));
        detail.EditValues["employee"].ShouldBe("emma@example.com");
        detail.EditValues["customer"].ShouldBe("buyer@contoso.example");
        detail.Collections.ShouldContain(collection => collection.Key == "salesLines");
        detail.Collections.ShouldContain(collection => collection.Key == "payments");
    }

    [Fact]
    public async Task CreateAsync_Creates_Sale_From_DateTime_Shortcuts()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var writeOptions = new DbContextOptionsBuilder<WriteDbContext>()
            .UseSqlite(connection)
            .Options;
        var readOptions = new DbContextOptionsBuilder<ReadDbContext>()
            .UseSqlite(connection)
            .Options;

        var employeeId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var before = DateTimeOffset.Now;

        await using (var arrangeContext = new WriteDbContext(writeOptions))
        {
            await arrangeContext.Database.EnsureCreatedAsync();
            arrangeContext.Employees.Add(new Employee(employeeId, "Emma", "emma@example.com", "444-44-4444"));
            arrangeContext.Customers.Add(new Customer(customerId, "Contoso", "buyer@contoso.example"));
            await arrangeContext.SaveChangesAsync();
        }

        await using var readContext = new ReadDbContext(readOptions);
        await using var writeContext = new WriteDbContext(writeOptions);
        IEntityWorkspaceBackendService service = new EntityWorkspaceBackendService(readContext, writeContext);

        await service.CreateAsync("sales", new Dictionary<string, string?>
        {
            ["when"] = "+1d",
            ["endWhen"] = "+1d",
            ["employee"] = "emma@example.com",
            ["customer"] = "buyer@contoso.example"
        });

        var after = DateTimeOffset.Now;

        await using var assertReadContext = new ReadDbContext(readOptions);
        var storedSale = await assertReadContext.Sales.SingleAsync();
        storedSale.When.ShouldBeGreaterThanOrEqualTo(before.AddDays(1));
        storedSale.When.ShouldBeLessThanOrEqualTo(after.AddDays(1));
    }

    [Fact]
    public async Task ExecuteCollectionActionAsync_Adds_SalesLine_And_Recalculates_Sale_Amount()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var writeOptions = new DbContextOptionsBuilder<WriteDbContext>()
            .UseSqlite(connection)
            .Options;
        var readOptions = new DbContextOptionsBuilder<ReadDbContext>()
            .UseSqlite(connection)
            .Options;

        var saleId = Guid.NewGuid();
        var employeeId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var itemId = Guid.NewGuid();

        await using (var arrangeContext = new WriteDbContext(writeOptions))
        {
            await arrangeContext.Database.EnsureCreatedAsync();
            arrangeContext.Employees.Add(new Employee(employeeId, "Emma", "emma@example.com", "444-44-4444"));
            arrangeContext.Customers.Add(new Customer(customerId, "Contoso", "buyer@contoso.example"));
            arrangeContext.Items.Add(new Item(itemId, "Widget"));
            arrangeContext.Sales.Add(new Sale(saleId, new DateTimeOffset(2026, 7, 3, 10, 0, 0, TimeSpan.Zero), employeeId, customerId));
            await arrangeContext.SaveChangesAsync();
        }

        await using var readContext = new ReadDbContext(readOptions);
        await using var writeContext = new WriteDbContext(writeOptions);
        IEntityWorkspaceBackendService service = new EntityWorkspaceBackendService(readContext, writeContext);

        var result = await service.ExecuteCollectionActionAsync("sales", saleId, "salesLines", "add", new Dictionary<string, string?>
        {
            ["item"] = itemId.ToString(),
            ["quantity"] = "2",
            ["unitPrice"] = "12.5"
        });

        result.Message.ShouldBe("Sales line added.");

        await using var assertReadContext = new ReadDbContext(readOptions);
        var storedSale = await assertReadContext.Sales.SingleAsync();
        var storedLine = await assertReadContext.SalesLines.SingleAsync();

        storedSale.Amount.ShouldBe(25m);
        storedLine.SaleId.ShouldBe(saleId);
        storedLine.ItemId.ShouldBe(itemId);
        storedLine.Quantity.ShouldBe(2m);
        storedLine.UnitPrice.ShouldBe(12.5m);
    }

    [Fact]
    public async Task ExecuteCollectionItemActionAsync_Removes_SalesLine()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var writeOptions = new DbContextOptionsBuilder<WriteDbContext>()
            .UseSqlite(connection)
            .Options;
        var readOptions = new DbContextOptionsBuilder<ReadDbContext>()
            .UseSqlite(connection)
            .Options;

        var saleId = Guid.NewGuid();
        var employeeId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        Guid lineId;

        await using (var arrangeContext = new WriteDbContext(writeOptions))
        {
            await arrangeContext.Database.EnsureCreatedAsync();
            var sale = new Sale(saleId, new DateTimeOffset(2026, 7, 3, 10, 0, 0, TimeSpan.Zero), employeeId, customerId);
            sale.AddSalesLine(itemId, 12.5m, 2m);
            lineId = sale.GetSalesLines().Single().Id.Value;

            arrangeContext.Employees.Add(new Employee(employeeId, "Emma", "emma@example.com", "444-44-4444"));
            arrangeContext.Customers.Add(new Customer(customerId, "Contoso", "buyer@contoso.example"));
            arrangeContext.Items.Add(new Item(itemId, "Widget"));
            arrangeContext.Sales.Add(sale);
            await arrangeContext.SaveChangesAsync();
        }

        await using var readContext = new ReadDbContext(readOptions);
        await using var writeContext = new WriteDbContext(writeOptions);
        IEntityWorkspaceBackendService service = new EntityWorkspaceBackendService(readContext, writeContext);

        var result = await service.ExecuteCollectionItemActionAsync("sales", saleId, "salesLines", lineId.ToString(), "remove");

        result.Message.ShouldBe("Sales line removed.");

        await using var assertReadContext = new ReadDbContext(readOptions);
        (await assertReadContext.SalesLines.CountAsync()).ShouldBe(0);
    }

    [Fact]
    public async Task CreateAsync_Creates_SalesOrder_With_Employee_And_Customer_Participations()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var writeOptions = new DbContextOptionsBuilder<WriteDbContext>()
            .UseSqlite(connection)
            .Options;
        var readOptions = new DbContextOptionsBuilder<ReadDbContext>()
            .UseSqlite(connection)
            .Options;

        var employeeId = Guid.NewGuid();
        var customerId = Guid.NewGuid();

        await using (var arrangeContext = new WriteDbContext(writeOptions))
        {
            await arrangeContext.Database.EnsureCreatedAsync();
            arrangeContext.Employees.Add(new Employee(employeeId, "Emma", "emma@example.com", "444-44-4444"));
            arrangeContext.Customers.Add(new Customer(customerId, "Contoso", "buyer@contoso.example"));
            await arrangeContext.SaveChangesAsync();
        }

        await using var readContext = new ReadDbContext(readOptions);
        await using var writeContext = new WriteDbContext(writeOptions);
        IEntityWorkspaceBackendService service = new EntityWorkspaceBackendService(readContext, writeContext);

        var result = await service.CreateAsync("sales-orders", new Dictionary<string, string?>
        {
            ["when"] = "2026-07-03T10:00:00+00:00",
            ["employee"] = "emma@example.com",
            ["customer"] = "buyer@contoso.example"
        });

        result.EntityId.ShouldNotBeNull();
        result.Message.ShouldBe("Sales order created.");

        await using var assertReadContext = new ReadDbContext(readOptions);
        var storedSalesOrder = await assertReadContext.SalesOrders.SingleAsync();
        storedSalesOrder.EmployeeId.ShouldBe(employeeId);
        storedSalesOrder.CustomerId.ShouldBe(customerId);
        storedSalesOrder.Amount.ShouldBeNull();
    }

    [Fact]
    public async Task GetAsync_Returns_SalesOrder_Detail_With_Employee_And_Customer()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var writeOptions = new DbContextOptionsBuilder<WriteDbContext>()
            .UseSqlite(connection)
            .Options;
        var readOptions = new DbContextOptionsBuilder<ReadDbContext>()
            .UseSqlite(connection)
            .Options;

        var salesOrderId = Guid.NewGuid();
        var employeeId = Guid.NewGuid();
        var customerId = Guid.NewGuid();

        await using (var arrangeContext = new WriteDbContext(writeOptions))
        {
            await arrangeContext.Database.EnsureCreatedAsync();
            arrangeContext.Employees.Add(new Employee(employeeId, "Emma", "emma@example.com", "444-44-4444"));
            arrangeContext.Customers.Add(new Customer(customerId, "Contoso", "buyer@contoso.example"));
            arrangeContext.Items.Add(new Item(Guid.NewGuid(), "Widget"));
            arrangeContext.SalesOrders.Add(new SalesOrder(salesOrderId, new DateTimeOffset(2026, 7, 3, 10, 0, 0, TimeSpan.Zero), employeeId, customerId, amount: 125.50m));
            await arrangeContext.SaveChangesAsync();
        }

        await using var readContext = new ReadDbContext(readOptions);
        await using var writeContext = new WriteDbContext(writeOptions);
        IEntityWorkspaceBackendService service = new EntityWorkspaceBackendService(readContext, writeContext);

        var detail = await service.GetAsync("sales-orders", salesOrderId);

        detail.ShouldNotBeNull();
        detail!.Summary.ShouldContain(property => property.Label == "Employee" && property.Value.Contains("emma@example.com"));
        detail.Summary.ShouldContain(property => property.Label == "Customer" && property.Value.Contains("buyer@contoso.example"));
        detail.EditValues["employee"].ShouldBe("emma@example.com");
        detail.EditValues["customer"].ShouldBe("buyer@contoso.example");
        detail.Collections.Single().Key.ShouldBe("salesOrderLines");
    }

    [Fact]
    public async Task ExecuteCollectionActionAsync_Adds_SalesOrderLine_And_Recalculates_SalesOrder_Amount()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var writeOptions = new DbContextOptionsBuilder<WriteDbContext>()
            .UseSqlite(connection)
            .Options;
        var readOptions = new DbContextOptionsBuilder<ReadDbContext>()
            .UseSqlite(connection)
            .Options;

        var salesOrderId = Guid.NewGuid();
        var employeeId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var itemId = Guid.NewGuid();

        await using (var arrangeContext = new WriteDbContext(writeOptions))
        {
            await arrangeContext.Database.EnsureCreatedAsync();
            arrangeContext.Employees.Add(new Employee(employeeId, "Emma", "emma@example.com", "444-44-4444"));
            arrangeContext.Customers.Add(new Customer(customerId, "Contoso", "buyer@contoso.example"));
            arrangeContext.Items.Add(new Item(itemId, "Widget"));
            arrangeContext.SalesOrders.Add(new SalesOrder(salesOrderId, new DateTimeOffset(2026, 7, 3, 10, 0, 0, TimeSpan.Zero), employeeId, customerId));
            await arrangeContext.SaveChangesAsync();
        }

        await using var readContext = new ReadDbContext(readOptions);
        await using var writeContext = new WriteDbContext(writeOptions);
        IEntityWorkspaceBackendService service = new EntityWorkspaceBackendService(readContext, writeContext);

        var result = await service.ExecuteCollectionActionAsync("sales-orders", salesOrderId, "salesOrderLines", "add", new Dictionary<string, string?>
        {
            ["item"] = itemId.ToString(),
            ["quantity"] = "2",
            ["unitPrice"] = "12.5"
        });

        result.Message.ShouldBe("Sales order line added.");

        await using var assertReadContext = new ReadDbContext(readOptions);
        var storedSalesOrder = await assertReadContext.SalesOrders.SingleAsync();
        var storedLine = await assertReadContext.SalesOrderLines.SingleAsync();

        storedSalesOrder.Amount.ShouldBe(25m);
        storedLine.SalesOrderId.ShouldBe(salesOrderId);
        storedLine.ItemId.ShouldBe(itemId);
        storedLine.Quantity.ShouldBe(2m);
        storedLine.UnitPrice.ShouldBe(12.5m);
    }

    [Fact]
    public async Task CreateAsync_Creates_ItContract()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var writeOptions = new DbContextOptionsBuilder<WriteDbContext>()
            .UseSqlite(connection)
            .Options;
        var readOptions = new DbContextOptionsBuilder<ReadDbContext>()
            .UseSqlite(connection)
            .Options;

        var employeeId = Guid.NewGuid();
        var vendorId = Guid.NewGuid();

        await using (var arrangeContext = new WriteDbContext(writeOptions))
        {
            await arrangeContext.Database.EnsureCreatedAsync();
            arrangeContext.Employees.Add(new Employee(employeeId, "Emma", "emma@example.com", "444-44-4444"));
            arrangeContext.Vendors.Add(new Vendor(vendorId, "Contoso Software", "vendor@contoso.example"));
            await arrangeContext.SaveChangesAsync();
        }

        await using var readContext = new ReadDbContext(readOptions);
        await using var writeContext = new WriteDbContext(writeOptions);
        IEntityWorkspaceBackendService service = new EntityWorkspaceBackendService(readContext, writeContext);

        var result = await service.CreateAsync("it-contracts", new Dictionary<string, string?>
        {
            ["serviceName"] = "Microsoft 365 E5",
            ["departmentCode"] = "FIN",
            ["startDate"] = "2026-01-15",
            ["endDate"] = "2026-03-14",
            ["prepaidAmount"] = "590",
            ["responsibleEmployee"] = "emma@example.com",
            ["vendor"] = "vendor@contoso.example"
        });

        result.EntityId.ShouldNotBeNull();
        result.Message.ShouldBe("IT contract created.");

        await using var assertReadContext = new ReadDbContext(readOptions);
        var storedContract = await assertReadContext.ItContracts.SingleAsync();
        storedContract.ServiceName.ShouldBe("Microsoft 365 E5");
        storedContract.DepartmentCode.ShouldBe("FIN");
        storedContract.ResponsibleEmployeeId.ShouldBe(employeeId);
        storedContract.VendorId.ShouldBe(vendorId);
        storedContract.Amount.ShouldBe(590m);
    }

    [Fact]
    public async Task GetAsync_Returns_Prepaid_It_Report_With_Department_Rows()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var writeOptions = new DbContextOptionsBuilder<WriteDbContext>()
            .UseSqlite(connection)
            .Options;
        var readOptions = new DbContextOptionsBuilder<ReadDbContext>()
            .UseSqlite(connection)
            .Options;

        var employeeId = Guid.NewGuid();
        var vendorId = Guid.NewGuid();
        var reportId = Guid.Parse("9D8F7E27-56AF-4A41-9965-65B02689AE43");

        await using (var arrangeContext = new WriteDbContext(writeOptions))
        {
            await arrangeContext.Database.EnsureCreatedAsync();
            arrangeContext.Employees.Add(new Employee(employeeId, "Emma", "emma@example.com", "444-44-4444"));
            arrangeContext.Vendors.Add(new Vendor(vendorId, "Contoso Software", "vendor@contoso.example"));
            arrangeContext.ItContracts.Add(new ItContract(Guid.NewGuid(), "Microsoft 365 E5", new DateTimeOffset(2026, 1, 15, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2026, 3, 14, 0, 0, 0, TimeSpan.Zero), 590m, "FIN", employeeId, vendorId));
            arrangeContext.ItContracts.Add(new ItContract(Guid.NewGuid(), "Adobe CC", new DateTimeOffset(2026, 2, 1, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2026, 2, 28, 0, 0, 0, TimeSpan.Zero), 280m, "HR", employeeId, vendorId));
            await arrangeContext.SaveChangesAsync();
        }

        await using var readContext = new ReadDbContext(readOptions);
        await using var writeContext = new WriteDbContext(writeOptions);
        IEntityWorkspaceBackendService service = new EntityWorkspaceBackendService(readContext, writeContext);

        var detail = await service.GetAsync("prepaid-it-report", reportId);

        detail.ShouldNotBeNull();
        detail!.Summary.ShouldContain(property => property.Label == "Days In Month" && property.Value == "Calculated");
        detail!.Summary.ShouldContain(property => property.Label == "Mermaid Diagram" && property.Value.Contains("xychart-beta"));
        detail.Collections.Single(collection => collection.Key == "reportOptions")
            .CreateAction.ShouldNotBeNull();
        detail.Collections.Single(collection => collection.Key == "departmentMonthlyExpenses")
            .Items.ShouldContain(item => item.Title.Contains("FIN") && item.Title.Contains("2026-02"));
    }

    [Fact]
    public async Task ExecuteCollectionActionAsync_For_Prepaid_It_Report_Returns_ThirtyDay_Report_Variant()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var writeOptions = new DbContextOptionsBuilder<WriteDbContext>()
            .UseSqlite(connection)
            .Options;
        var readOptions = new DbContextOptionsBuilder<ReadDbContext>()
            .UseSqlite(connection)
            .Options;

        var employeeId = Guid.NewGuid();
        var vendorId = Guid.NewGuid();
        var calculatedReportId = Guid.Parse("9D8F7E27-56AF-4A41-9965-65B02689AE43");

        await using (var arrangeContext = new WriteDbContext(writeOptions))
        {
            await arrangeContext.Database.EnsureCreatedAsync();
            arrangeContext.Employees.Add(new Employee(employeeId, "Emma", "emma@example.com", "444-44-4444"));
            arrangeContext.Vendors.Add(new Vendor(vendorId, "Contoso Software", "vendor@contoso.example"));
            arrangeContext.ItContracts.Add(new ItContract(Guid.NewGuid(), "Leap Contract", new DateTimeOffset(2024, 1, 15, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2024, 4, 14, 0, 0, 0, TimeSpan.Zero), 900m, "FIN", employeeId, vendorId));
            await arrangeContext.SaveChangesAsync();
        }

        await using var readContext = new ReadDbContext(readOptions);
        await using var writeContext = new WriteDbContext(writeOptions);
        IEntityWorkspaceBackendService service = new EntityWorkspaceBackendService(readContext, writeContext);

        var actionResult = await service.ExecuteCollectionActionAsync("prepaid-it-report", calculatedReportId, "reportOptions", "generate", new Dictionary<string, string?>
        {
            ["daysInMonth"] = "30days"
        });

        actionResult.EntityId.ShouldNotBeNull();
        actionResult.Message.ShouldContain("30 days");

        var detail = await service.GetAsync("prepaid-it-report", actionResult.EntityId.Value);

        detail.ShouldNotBeNull();
        detail!.Summary.ShouldContain(property => property.Label == "Days In Month" && property.Value == "30 days");

        var february = detail.Collections
            .Single(collection => collection.Key == "departmentMonthlyExpenses")
            .Items
            .Single(item => item.Title.Contains("FIN") && item.Title.Contains("2024-02"));
        var march = detail.Collections
            .Single(collection => collection.Key == "departmentMonthlyExpenses")
            .Items
            .Single(item => item.Title.Contains("FIN") && item.Title.Contains("2024-03"));

        february.Meta.Single(meta => meta.Label == "Amount").Value.ShouldBe("300.00");
        march.Meta.Single(meta => meta.Label == "Amount").Value.ShouldBe("300.00");
    }
}
