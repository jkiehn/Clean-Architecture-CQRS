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
