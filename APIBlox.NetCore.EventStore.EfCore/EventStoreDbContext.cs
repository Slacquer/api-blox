using System;
using System.IO;
using APIBlox.NetCore.Documents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;

namespace APIBlox.NetCore
{
    internal class EventStoreDbContext : DbContext
    {
        public EventStoreDbContext(DbContextOptions<EventStoreDbContext> options)
            : base(options)
        {
        }

        public DbSet<EventStoreDocument> Documents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var eventStoreDocumentMap = new EventStoreDocumentMap(modelBuilder.Entity<EventStoreDocument>());

            eventStoreDocumentMap.Map();
        }
    }

    internal class EventStoreDbContextFactory : DesignTimeDbContextFactoryBase<EventStoreDbContext>
    {
        protected override EventStoreDbContext CreateNewInstance(DbContextOptions<EventStoreDbContext> options)
        {
            return new EventStoreDbContext(options);
        }
    }

    internal abstract class DesignTimeDbContextFactoryBase<TContext> :
        IDesignTimeDbContextFactory<TContext> where TContext : DbContext
    {
        public TContext CreateDbContext(string[] args)
        {
            return Create(
                Directory.GetCurrentDirectory(),
                Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"));
        }
        protected abstract TContext CreateNewInstance(
            DbContextOptions<TContext> options);
        

        private TContext Create(string basePath, string environmentName)
        {
            var builder = new ConfigurationBuilder()

                .SetBasePath(basePath)
                .AddJsonFile("appSettings.json")
                .AddJsonFile($"appSettings.{environmentName}.json", true);

            var config = builder.Build();

            var connStr = config.GetConnectionString("default");

            if (string.IsNullOrWhiteSpace(connStr))
            {
                throw new InvalidOperationException(
                    "Could not find a connection string named 'default'.");
            }

            return Create(connStr);

        }

        private TContext Create(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException(
             $"{nameof(connectionString)} is null or empty.",
             nameof(connectionString));

            var optionsBuilder =
                 new DbContextOptionsBuilder<TContext>();

            Console.WriteLine(
                "MyDesignTimeDbContextFactory.Create(string): Connection string: {0}",
                connectionString);

            optionsBuilder.UseSqlServer(connectionString);

            DbContextOptions<TContext> options = optionsBuilder.Options;

            return CreateNewInstance(options);
        }

    }
}
