using System.Data;
using CleanArchitectureCQRS.Infrastructure.EF.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitectureCQRS.Infrastructure.EF;

public static class SqlSchemaInitializer
{
    private const string MigrationHistoryTableName = "__EFMigrationsHistory";
    private const string MigrationHistoryCreateSql = """
        IF OBJECT_ID(N'[dbo].[__EFMigrationsHistory]', N'U') IS NULL
        BEGIN
            CREATE TABLE [dbo].[__EFMigrationsHistory]
            (
                [MigrationId] nvarchar(150) NOT NULL,
                [ProductVersion] nvarchar(32) NOT NULL,
                CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
            );
        END
        """;

    private const string ExistingDatabaseBridgeSql = """
        IF OBJECT_ID(N'[SampleEntity].[Cash]', N'U') IS NULL
        BEGIN
            CREATE TABLE [SampleEntity].[Cash]
            (
                [Id] uniqueidentifier NOT NULL,
                [Name] nvarchar(max) NOT NULL,
                [Version] int NOT NULL CONSTRAINT [DF_Cash_Version] DEFAULT (0),
                CONSTRAINT [PK_Cash] PRIMARY KEY ([Id])
            );
        END

        IF OBJECT_ID(N'[SampleEntity].[CustomerPayments]', N'U') IS NULL
        BEGIN
            CREATE TABLE [SampleEntity].[CustomerPayments]
            (
                [Id] uniqueidentifier NOT NULL,
                [When] datetimeoffset NOT NULL,
                [EndWhen] datetimeoffset NULL,
                [Amount] decimal(18,2) NULL,
                [ExternalParticipationId] uniqueidentifier NOT NULL,
                [CustomerId] uniqueidentifier NOT NULL,
                [CashFlowId] uniqueidentifier NOT NULL,
                [CashResourceId] uniqueidentifier NOT NULL,
                [Version] int NOT NULL CONSTRAINT [DF_CustomerPayments_Version] DEFAULT (0),
                CONSTRAINT [PK_CustomerPayments] PRIMARY KEY ([Id])
            );
        END

        IF OBJECT_ID(N'[SampleEntity].[CustomerPaymentCashFlows]', N'U') IS NULL
        BEGIN
            CREATE TABLE [SampleEntity].[CustomerPaymentCashFlows]
            (
                [Id] uniqueidentifier NOT NULL,
                [OccurrentEndId] uniqueidentifier NOT NULL,
                [ResourceEndId] uniqueidentifier NOT NULL,
                [CustomerPaymentId] uniqueidentifier NOT NULL,
                [CashResourceId] uniqueidentifier NOT NULL,
                [Version] int NOT NULL CONSTRAINT [DF_CustomerPaymentCashFlows_Version] DEFAULT (0),
                CONSTRAINT [PK_CustomerPaymentCashFlows] PRIMARY KEY ([Id]),
                CONSTRAINT [FK_CustomerPaymentCashFlows_CustomerPayments_CustomerPaymentId]
                    FOREIGN KEY ([CustomerPaymentId]) REFERENCES [SampleEntity].[CustomerPayments]([Id]) ON DELETE CASCADE
            );
        END

        IF OBJECT_ID(N'[SampleEntity].[PaysFor]', N'U') IS NULL
        BEGIN
            CREATE TABLE [SampleEntity].[PaysFor]
            (
                [Id] uniqueidentifier NOT NULL,
                [SaleId] uniqueidentifier NOT NULL,
                [CustomerPaymentId] uniqueidentifier NOT NULL,
                [Version] int NOT NULL CONSTRAINT [DF_PaysFor_Version] DEFAULT (0),
                CONSTRAINT [PK_PaysFor] PRIMARY KEY ([Id])
            );
        END

        IF NOT EXISTS
        (
            SELECT 1
            FROM sys.indexes
            WHERE name = N'IX_PaysFor_SaleId_CustomerPaymentId'
              AND object_id = OBJECT_ID(N'[SampleEntity].[PaysFor]')
        )
        BEGIN
            CREATE UNIQUE INDEX [IX_PaysFor_SaleId_CustomerPaymentId]
                ON [SampleEntity].[PaysFor]([SaleId], [CustomerPaymentId]);
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

        if (!await HasAnyUserTableAsync(writeDbContext))
        {
            await writeDbContext.Database.MigrateAsync();
            return;
        }

        var migrationsAssembly = writeDbContext.GetService<IMigrationsAssembly>();
        var latestMigrationId = migrationsAssembly.Migrations.Keys.OrderBy(key => key).LastOrDefault();

        if (string.IsNullOrWhiteSpace(latestMigrationId))
        {
            throw new InvalidOperationException("No EF migrations were found for WriteDbContext.");
        }

        var hasHistoryTable = await TableExistsAsync(writeDbContext, "dbo", MigrationHistoryTableName);

        if (!hasHistoryTable)
        {
            await writeDbContext.Database.ExecuteSqlRawAsync(ExistingDatabaseBridgeSql);
            await writeDbContext.Database.ExecuteSqlRawAsync(MigrationHistoryCreateSql);
            await InsertMigrationHistoryAsync(writeDbContext, latestMigrationId);
        }

        await writeDbContext.Database.MigrateAsync();
    }

    private static async Task<bool> HasAnyUserTableAsync(DbContext dbContext)
    {
        await using var command = dbContext.Database.GetDbConnection().CreateCommand();
        command.CommandText = """
            SELECT TOP (1) 1
            FROM INFORMATION_SCHEMA.TABLES
            WHERE TABLE_TYPE = 'BASE TABLE'
              AND TABLE_SCHEMA NOT IN ('sys')
            """;

        if (command.Connection!.State != ConnectionState.Open)
        {
            await command.Connection.OpenAsync();
        }

        var result = await command.ExecuteScalarAsync();
        return result is not null && result != DBNull.Value;
    }

    private static async Task<bool> TableExistsAsync(DbContext dbContext, string schema, string tableName)
    {
        await using var command = dbContext.Database.GetDbConnection().CreateCommand();
        command.CommandText = """
            SELECT 1
            FROM INFORMATION_SCHEMA.TABLES
            WHERE TABLE_SCHEMA = @schema
              AND TABLE_NAME = @tableName
            """;

        var schemaParameter = command.CreateParameter();
        schemaParameter.ParameterName = "@schema";
        schemaParameter.Value = schema;
        command.Parameters.Add(schemaParameter);

        var tableParameter = command.CreateParameter();
        tableParameter.ParameterName = "@tableName";
        tableParameter.Value = tableName;
        command.Parameters.Add(tableParameter);

        if (command.Connection!.State != ConnectionState.Open)
        {
            await command.Connection.OpenAsync();
        }

        var result = await command.ExecuteScalarAsync();
        return result is not null && result != DBNull.Value;
    }

    private static async Task InsertMigrationHistoryAsync(DbContext dbContext, string migrationId)
    {
        await using var command = dbContext.Database.GetDbConnection().CreateCommand();
        command.CommandText = """
            IF NOT EXISTS (SELECT 1 FROM [dbo].[__EFMigrationsHistory] WHERE [MigrationId] = @migrationId)
            BEGIN
                INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
                VALUES (@migrationId, @productVersion);
            END
            """;

        var migrationParameter = command.CreateParameter();
        migrationParameter.ParameterName = "@migrationId";
        migrationParameter.Value = migrationId;
        command.Parameters.Add(migrationParameter);

        var versionParameter = command.CreateParameter();
        versionParameter.ParameterName = "@productVersion";
        versionParameter.Value = typeof(DbContext).Assembly.GetName().Version?.ToString(3) ?? "10.0.0";
        command.Parameters.Add(versionParameter);

        if (command.Connection!.State != ConnectionState.Open)
        {
            await command.Connection.OpenAsync();
        }

        await command.ExecuteNonQueryAsync();
    }
}
