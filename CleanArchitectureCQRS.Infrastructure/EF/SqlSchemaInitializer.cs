using CleanArchitectureCQRS.Infrastructure.EF.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitectureCQRS.Infrastructure.EF;

public static class SqlSchemaInitializer
{
    private const string EnsureAgentTablesSql = """
        IF SCHEMA_ID(N'SampleEntity') IS NULL
        BEGIN
            EXEC(N'CREATE SCHEMA [SampleEntity]');
        END

        IF OBJECT_ID(N'[SampleEntity].[Customers]', N'U') IS NULL
        BEGIN
            CREATE TABLE [SampleEntity].[Customers]
            (
                [Id] uniqueidentifier NOT NULL,
                [Name] nvarchar(max) NOT NULL,
                [Email] nvarchar(max) NOT NULL,
                [Version] int NOT NULL CONSTRAINT [DF_Customers_Version] DEFAULT (0),
                CONSTRAINT [PK_Customers] PRIMARY KEY ([Id])
            );
        END
        ELSE IF COL_LENGTH(N'[SampleEntity].[Customers]', N'Version') IS NULL
        BEGIN
            ALTER TABLE [SampleEntity].[Customers]
            ADD [Version] int NOT NULL CONSTRAINT [DF_Customers_Version] DEFAULT (0);
        END

        IF OBJECT_ID(N'[SampleEntity].[Vendors]', N'U') IS NULL
        BEGIN
            CREATE TABLE [SampleEntity].[Vendors]
            (
                [Id] uniqueidentifier NOT NULL,
                [Name] nvarchar(max) NOT NULL,
                [Email] nvarchar(max) NOT NULL,
                [Version] int NOT NULL CONSTRAINT [DF_Vendors_Version] DEFAULT (0),
                CONSTRAINT [PK_Vendors] PRIMARY KEY ([Id])
            );
        END
        ELSE IF COL_LENGTH(N'[SampleEntity].[Vendors]', N'Version') IS NULL
        BEGIN
            ALTER TABLE [SampleEntity].[Vendors]
            ADD [Version] int NOT NULL CONSTRAINT [DF_Vendors_Version] DEFAULT (0);
        END

        IF OBJECT_ID(N'[SampleEntity].[Employees]', N'U') IS NULL
        BEGIN
            CREATE TABLE [SampleEntity].[Employees]
            (
                [Id] uniqueidentifier NOT NULL,
                [Name] nvarchar(max) NOT NULL,
                [Email] nvarchar(max) NOT NULL,
                [SocialSecurityNumber] nvarchar(max) NOT NULL,
                [Version] int NOT NULL CONSTRAINT [DF_Employees_Version] DEFAULT (0),
                CONSTRAINT [PK_Employees] PRIMARY KEY ([Id])
            );
        END
        ELSE IF COL_LENGTH(N'[SampleEntity].[Employees]', N'Version') IS NULL
        BEGIN
            ALTER TABLE [SampleEntity].[Employees]
            ADD [Version] int NOT NULL CONSTRAINT [DF_Employees_Version] DEFAULT (0);
        END

        IF OBJECT_ID(N'[SampleEntity].[Employees]', N'U') IS NOT NULL
            AND COL_LENGTH(N'[SampleEntity].[Employees]', N'SocialSecurityNumber') IS NULL
        BEGIN
            ALTER TABLE [SampleEntity].[Employees]
            ADD [SocialSecurityNumber] nvarchar(max) NOT NULL CONSTRAINT [DF_Employees_SocialSecurityNumber] DEFAULT (N'');
        END

        IF OBJECT_ID(N'[SampleEntity].[Items]', N'U') IS NULL
        BEGIN
            CREATE TABLE [SampleEntity].[Items]
            (
                [Id] uniqueidentifier NOT NULL,
                [Name] nvarchar(max) NOT NULL,
                [Version] int NOT NULL CONSTRAINT [DF_Items_Version] DEFAULT (0),
                CONSTRAINT [PK_Items] PRIMARY KEY ([Id])
            );
        END
        ELSE IF COL_LENGTH(N'[SampleEntity].[Items]', N'Version') IS NULL
        BEGIN
            ALTER TABLE [SampleEntity].[Items]
            ADD [Version] int NOT NULL CONSTRAINT [DF_Items_Version] DEFAULT (0);
        END

        IF OBJECT_ID(N'[SampleEntity].[Sales]', N'U') IS NULL
        BEGIN
            CREATE TABLE [SampleEntity].[Sales]
            (
                [Id] uniqueidentifier NOT NULL,
                [When] datetimeoffset NOT NULL,
                [EndWhen] datetimeoffset NULL,
                [Amount] decimal(18,2) NULL,
                [InternalParticipationId] uniqueidentifier NOT NULL,
                [EmployeeId] uniqueidentifier NOT NULL,
                [ExternalParticipationId] uniqueidentifier NOT NULL,
                [CustomerId] uniqueidentifier NOT NULL,
                [Version] int NOT NULL CONSTRAINT [DF_Sales_Version] DEFAULT (0),
                CONSTRAINT [PK_Sales] PRIMARY KEY ([Id])
            );
        END
        ELSE IF COL_LENGTH(N'[SampleEntity].[Sales]', N'Version') IS NULL
        BEGIN
            ALTER TABLE [SampleEntity].[Sales]
            ADD [Version] int NOT NULL CONSTRAINT [DF_Sales_Version] DEFAULT (0);
        END

        IF OBJECT_ID(N'[SampleEntity].[Sales]', N'U') IS NOT NULL
            AND COL_LENGTH(N'[SampleEntity].[Sales]', N'When') IS NULL
        BEGIN
            ALTER TABLE [SampleEntity].[Sales]
            ADD [When] datetimeoffset NOT NULL CONSTRAINT [DF_Sales_When] DEFAULT (SYSDATETIMEOFFSET());
        END

        IF OBJECT_ID(N'[SampleEntity].[Sales]', N'U') IS NOT NULL
            AND COL_LENGTH(N'[SampleEntity].[Sales]', N'EndWhen') IS NULL
        BEGIN
            ALTER TABLE [SampleEntity].[Sales]
            ADD [EndWhen] datetimeoffset NULL;
        END

        IF OBJECT_ID(N'[SampleEntity].[Sales]', N'U') IS NOT NULL
            AND COL_LENGTH(N'[SampleEntity].[Sales]', N'Amount') IS NULL
        BEGIN
            ALTER TABLE [SampleEntity].[Sales]
            ADD [Amount] decimal(18,2) NULL;
        END

        IF OBJECT_ID(N'[SampleEntity].[Sales]', N'U') IS NOT NULL
            AND COL_LENGTH(N'[SampleEntity].[Sales]', N'InternalParticipationId') IS NULL
        BEGIN
            ALTER TABLE [SampleEntity].[Sales]
            ADD [InternalParticipationId] uniqueidentifier NOT NULL CONSTRAINT [DF_Sales_InternalParticipationId] DEFAULT (NEWID());
        END

        IF OBJECT_ID(N'[SampleEntity].[Sales]', N'U') IS NOT NULL
            AND COL_LENGTH(N'[SampleEntity].[Sales]', N'EmployeeId') IS NULL
        BEGIN
            ALTER TABLE [SampleEntity].[Sales]
            ADD [EmployeeId] uniqueidentifier NOT NULL CONSTRAINT [DF_Sales_EmployeeId] DEFAULT ('00000000-0000-0000-0000-000000000001');
        END

        IF OBJECT_ID(N'[SampleEntity].[Sales]', N'U') IS NOT NULL
            AND COL_LENGTH(N'[SampleEntity].[Sales]', N'ExternalParticipationId') IS NULL
        BEGIN
            ALTER TABLE [SampleEntity].[Sales]
            ADD [ExternalParticipationId] uniqueidentifier NOT NULL CONSTRAINT [DF_Sales_ExternalParticipationId] DEFAULT (NEWID());
        END

        IF OBJECT_ID(N'[SampleEntity].[Sales]', N'U') IS NOT NULL
            AND COL_LENGTH(N'[SampleEntity].[Sales]', N'CustomerId') IS NULL
        BEGIN
            ALTER TABLE [SampleEntity].[Sales]
            ADD [CustomerId] uniqueidentifier NOT NULL CONSTRAINT [DF_Sales_CustomerId] DEFAULT ('00000000-0000-0000-0000-000000000001');
        END

        IF OBJECT_ID(N'[SampleEntity].[SalesLines]', N'U') IS NULL
        BEGIN
            CREATE TABLE [SampleEntity].[SalesLines]
            (
                [Id] uniqueidentifier NOT NULL,
                [OccurrentEndId] uniqueidentifier NOT NULL,
                [ResourceEndId] uniqueidentifier NOT NULL,
                [SaleId] uniqueidentifier NOT NULL,
                [ItemId] uniqueidentifier NOT NULL,
                [UnitPrice] decimal(18,2) NOT NULL,
                [Quantity] decimal(18,2) NOT NULL,
                [Version] int NOT NULL CONSTRAINT [DF_SalesLines_Version] DEFAULT (0),
                CONSTRAINT [PK_SalesLines] PRIMARY KEY ([Id]),
                CONSTRAINT [FK_SalesLines_Sales_SaleId] FOREIGN KEY ([SaleId]) REFERENCES [SampleEntity].[Sales]([Id]) ON DELETE CASCADE
            );
        END
        ELSE IF COL_LENGTH(N'[SampleEntity].[SalesLines]', N'Version') IS NULL
        BEGIN
            ALTER TABLE [SampleEntity].[SalesLines]
            ADD [Version] int NOT NULL CONSTRAINT [DF_SalesLines_Version] DEFAULT (0);
        END

        IF OBJECT_ID(N'[SampleEntity].[SalesLines]', N'U') IS NOT NULL
            AND COL_LENGTH(N'[SampleEntity].[SalesLines]', N'OccurrentEndId') IS NULL
        BEGIN
            ALTER TABLE [SampleEntity].[SalesLines]
            ADD [OccurrentEndId] uniqueidentifier NOT NULL CONSTRAINT [DF_SalesLines_OccurrentEndId] DEFAULT (NEWID());
        END

        IF OBJECT_ID(N'[SampleEntity].[SalesLines]', N'U') IS NOT NULL
            AND COL_LENGTH(N'[SampleEntity].[SalesLines]', N'ResourceEndId') IS NULL
        BEGIN
            ALTER TABLE [SampleEntity].[SalesLines]
            ADD [ResourceEndId] uniqueidentifier NOT NULL CONSTRAINT [DF_SalesLines_ResourceEndId] DEFAULT (NEWID());
        END

        IF OBJECT_ID(N'[SampleEntity].[SalesLines]', N'U') IS NOT NULL
            AND COL_LENGTH(N'[SampleEntity].[SalesLines]', N'SaleId') IS NULL
        BEGIN
            ALTER TABLE [SampleEntity].[SalesLines]
            ADD [SaleId] uniqueidentifier NOT NULL CONSTRAINT [DF_SalesLines_SaleId] DEFAULT ('00000000-0000-0000-0000-000000000001');
        END

        IF OBJECT_ID(N'[SampleEntity].[SalesLines]', N'U') IS NOT NULL
            AND COL_LENGTH(N'[SampleEntity].[SalesLines]', N'ItemId') IS NULL
        BEGIN
            ALTER TABLE [SampleEntity].[SalesLines]
            ADD [ItemId] uniqueidentifier NOT NULL CONSTRAINT [DF_SalesLines_ItemId] DEFAULT ('00000000-0000-0000-0000-000000000001');
        END

        IF OBJECT_ID(N'[SampleEntity].[SalesLines]', N'U') IS NOT NULL
            AND COL_LENGTH(N'[SampleEntity].[SalesLines]', N'UnitPrice') IS NULL
        BEGIN
            ALTER TABLE [SampleEntity].[SalesLines]
            ADD [UnitPrice] decimal(18,2) NOT NULL CONSTRAINT [DF_SalesLines_UnitPrice] DEFAULT (0);
        END

        IF OBJECT_ID(N'[SampleEntity].[SalesLines]', N'U') IS NOT NULL
            AND COL_LENGTH(N'[SampleEntity].[SalesLines]', N'Quantity') IS NULL
        BEGIN
            ALTER TABLE [SampleEntity].[SalesLines]
            ADD [Quantity] decimal(18,2) NOT NULL CONSTRAINT [DF_SalesLines_Quantity] DEFAULT (1);
        END

        IF OBJECT_ID(N'[SampleEntity].[SalesOrders]', N'U') IS NULL
        BEGIN
            CREATE TABLE [SampleEntity].[SalesOrders]
            (
                [Id] uniqueidentifier NOT NULL,
                [When] datetimeoffset NOT NULL,
                [EndWhen] datetimeoffset NULL,
                [Amount] decimal(18,2) NULL,
                [InternalParticipationId] uniqueidentifier NOT NULL,
                [EmployeeId] uniqueidentifier NOT NULL,
                [ExternalParticipationId] uniqueidentifier NOT NULL,
                [CustomerId] uniqueidentifier NOT NULL,
                [Version] int NOT NULL CONSTRAINT [DF_SalesOrders_Version] DEFAULT (0),
                CONSTRAINT [PK_SalesOrders] PRIMARY KEY ([Id])
            );
        END
        ELSE IF COL_LENGTH(N'[SampleEntity].[SalesOrders]', N'Version') IS NULL
        BEGIN
            ALTER TABLE [SampleEntity].[SalesOrders]
            ADD [Version] int NOT NULL CONSTRAINT [DF_SalesOrders_Version] DEFAULT (0);
        END

        IF OBJECT_ID(N'[SampleEntity].[SalesOrders]', N'U') IS NOT NULL
            AND COL_LENGTH(N'[SampleEntity].[SalesOrders]', N'When') IS NULL
        BEGIN
            ALTER TABLE [SampleEntity].[SalesOrders]
            ADD [When] datetimeoffset NOT NULL CONSTRAINT [DF_SalesOrders_When] DEFAULT (SYSDATETIMEOFFSET());
        END

        IF OBJECT_ID(N'[SampleEntity].[SalesOrders]', N'U') IS NOT NULL
            AND COL_LENGTH(N'[SampleEntity].[SalesOrders]', N'EndWhen') IS NULL
        BEGIN
            ALTER TABLE [SampleEntity].[SalesOrders]
            ADD [EndWhen] datetimeoffset NULL;
        END

        IF OBJECT_ID(N'[SampleEntity].[SalesOrders]', N'U') IS NOT NULL
            AND COL_LENGTH(N'[SampleEntity].[SalesOrders]', N'Amount') IS NULL
        BEGIN
            ALTER TABLE [SampleEntity].[SalesOrders]
            ADD [Amount] decimal(18,2) NULL;
        END

        IF OBJECT_ID(N'[SampleEntity].[SalesOrders]', N'U') IS NOT NULL
            AND COL_LENGTH(N'[SampleEntity].[SalesOrders]', N'InternalParticipationId') IS NULL
        BEGIN
            ALTER TABLE [SampleEntity].[SalesOrders]
            ADD [InternalParticipationId] uniqueidentifier NOT NULL CONSTRAINT [DF_SalesOrders_InternalParticipationId] DEFAULT (NEWID());
        END

        IF OBJECT_ID(N'[SampleEntity].[SalesOrders]', N'U') IS NOT NULL
            AND COL_LENGTH(N'[SampleEntity].[SalesOrders]', N'EmployeeId') IS NULL
        BEGIN
            ALTER TABLE [SampleEntity].[SalesOrders]
            ADD [EmployeeId] uniqueidentifier NOT NULL CONSTRAINT [DF_SalesOrders_EmployeeId] DEFAULT ('00000000-0000-0000-0000-000000000001');
        END

        IF OBJECT_ID(N'[SampleEntity].[SalesOrders]', N'U') IS NOT NULL
            AND COL_LENGTH(N'[SampleEntity].[SalesOrders]', N'ExternalParticipationId') IS NULL
        BEGIN
            ALTER TABLE [SampleEntity].[SalesOrders]
            ADD [ExternalParticipationId] uniqueidentifier NOT NULL CONSTRAINT [DF_SalesOrders_ExternalParticipationId] DEFAULT (NEWID());
        END

        IF OBJECT_ID(N'[SampleEntity].[SalesOrders]', N'U') IS NOT NULL
            AND COL_LENGTH(N'[SampleEntity].[SalesOrders]', N'CustomerId') IS NULL
        BEGIN
            ALTER TABLE [SampleEntity].[SalesOrders]
            ADD [CustomerId] uniqueidentifier NOT NULL CONSTRAINT [DF_SalesOrders_CustomerId] DEFAULT ('00000000-0000-0000-0000-000000000001');
        END

        IF OBJECT_ID(N'[SampleEntity].[SalesOrderLines]', N'U') IS NULL
        BEGIN
            CREATE TABLE [SampleEntity].[SalesOrderLines]
            (
                [Id] uniqueidentifier NOT NULL,
                [OccurrentEndId] uniqueidentifier NOT NULL,
                [ResourceEndId] uniqueidentifier NOT NULL,
                [SalesOrderId] uniqueidentifier NOT NULL,
                [ItemId] uniqueidentifier NOT NULL,
                [UnitPrice] decimal(18,2) NOT NULL,
                [Quantity] decimal(18,2) NOT NULL,
                [Version] int NOT NULL CONSTRAINT [DF_SalesOrderLines_Version] DEFAULT (0),
                CONSTRAINT [PK_SalesOrderLines] PRIMARY KEY ([Id]),
                CONSTRAINT [FK_SalesOrderLines_SalesOrders_SalesOrderId] FOREIGN KEY ([SalesOrderId]) REFERENCES [SampleEntity].[SalesOrders]([Id]) ON DELETE CASCADE
            );
        END
        ELSE IF COL_LENGTH(N'[SampleEntity].[SalesOrderLines]', N'Version') IS NULL
        BEGIN
            ALTER TABLE [SampleEntity].[SalesOrderLines]
            ADD [Version] int NOT NULL CONSTRAINT [DF_SalesOrderLines_Version] DEFAULT (0);
        END

        IF OBJECT_ID(N'[SampleEntity].[SalesOrderLines]', N'U') IS NOT NULL
            AND COL_LENGTH(N'[SampleEntity].[SalesOrderLines]', N'OccurrentEndId') IS NULL
        BEGIN
            ALTER TABLE [SampleEntity].[SalesOrderLines]
            ADD [OccurrentEndId] uniqueidentifier NOT NULL CONSTRAINT [DF_SalesOrderLines_OccurrentEndId] DEFAULT (NEWID());
        END

        IF OBJECT_ID(N'[SampleEntity].[SalesOrderLines]', N'U') IS NOT NULL
            AND COL_LENGTH(N'[SampleEntity].[SalesOrderLines]', N'ResourceEndId') IS NULL
        BEGIN
            ALTER TABLE [SampleEntity].[SalesOrderLines]
            ADD [ResourceEndId] uniqueidentifier NOT NULL CONSTRAINT [DF_SalesOrderLines_ResourceEndId] DEFAULT (NEWID());
        END

        IF OBJECT_ID(N'[SampleEntity].[SalesOrderLines]', N'U') IS NOT NULL
            AND COL_LENGTH(N'[SampleEntity].[SalesOrderLines]', N'SalesOrderId') IS NULL
        BEGIN
            ALTER TABLE [SampleEntity].[SalesOrderLines]
            ADD [SalesOrderId] uniqueidentifier NOT NULL CONSTRAINT [DF_SalesOrderLines_SalesOrderId] DEFAULT ('00000000-0000-0000-0000-000000000001');
        END

        IF OBJECT_ID(N'[SampleEntity].[SalesOrderLines]', N'U') IS NOT NULL
            AND COL_LENGTH(N'[SampleEntity].[SalesOrderLines]', N'ItemId') IS NULL
        BEGIN
            ALTER TABLE [SampleEntity].[SalesOrderLines]
            ADD [ItemId] uniqueidentifier NOT NULL CONSTRAINT [DF_SalesOrderLines_ItemId] DEFAULT ('00000000-0000-0000-0000-000000000001');
        END

        IF OBJECT_ID(N'[SampleEntity].[SalesOrderLines]', N'U') IS NOT NULL
            AND COL_LENGTH(N'[SampleEntity].[SalesOrderLines]', N'UnitPrice') IS NULL
        BEGIN
            ALTER TABLE [SampleEntity].[SalesOrderLines]
            ADD [UnitPrice] decimal(18,2) NOT NULL CONSTRAINT [DF_SalesOrderLines_UnitPrice] DEFAULT (0);
        END

        IF OBJECT_ID(N'[SampleEntity].[SalesOrderLines]', N'U') IS NOT NULL
            AND COL_LENGTH(N'[SampleEntity].[SalesOrderLines]', N'Quantity') IS NULL
        BEGIN
            ALTER TABLE [SampleEntity].[SalesOrderLines]
            ADD [Quantity] decimal(18,2) NOT NULL CONSTRAINT [DF_SalesOrderLines_Quantity] DEFAULT (1);
        END
        """;

    public static async Task EnsureAsync(this IServiceProvider serviceProvider)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var writeDbContext = scope.ServiceProvider.GetRequiredService<WriteDbContext>();

        if (!writeDbContext.Database.IsSqlServer())
        {
            return;
        }

        await writeDbContext.Database.ExecuteSqlRawAsync(EnsureAgentTablesSql);
    }
}
