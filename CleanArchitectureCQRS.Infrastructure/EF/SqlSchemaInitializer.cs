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
