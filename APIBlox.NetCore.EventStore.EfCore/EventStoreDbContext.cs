using System;
using System.IO;
using APIBlox.NetCore.Extensions;
using APIBlox.NetCore.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace APIBlox.NetCore
{
    internal sealed class EventStoreDbContext : DbContext
    {
        public EventStoreDbContext(DbContextOptions<EventStoreDbContext> options)
            : base(options)
        {
        }

        public DbSet<DocEx> Documents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var eventStoreDocumentMap = new EventStoreDocumentMap(modelBuilder.Entity<DocEx>());

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
        IDesignTimeDbContextFactory<TContext>
        where TContext : DbContext
    {
        public TContext CreateDbContext(string[] args)
        {
            return Create(Directory.GetCurrentDirectory(), Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"));
        }

        protected abstract TContext CreateNewInstance(DbContextOptions<TContext> options);

        private TContext Create(string basePath, string environmentName)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appSettings.json")
                .AddJsonFile($"appSettings.{environmentName}.json", true);

            var config = builder.Build().GetSection(nameof(EfCoreSqlOptions));

            var es = config.Get<EfCoreSqlOptions>();

            if (es is null)
                throw new ArgumentException(
                    $"In order to use the {nameof(EfCoreSqlOptions)} you " +
                    $"will need to have an {nameof(EfCoreSqlOptions)} configuration entry."
                );

            return Create(es);
        }

        private TContext Create(EfCoreSqlOptions efCoreOptions)
        {
            if (efCoreOptions.CnnString.IsEmptyNullOrWhiteSpace())
                throw new ArgumentException($"{nameof(efCoreOptions.CnnString)} is null or empty.", nameof(efCoreOptions));

            var optionsBuilder = new DbContextOptionsBuilder<TContext>();

            if (Environment.UserInteractive)
                Console.WriteLine($"---------------CNNSTR: {efCoreOptions.CnnString}");

            optionsBuilder.UseSqlServer(efCoreOptions.CnnString);

            return CreateNewInstance(optionsBuilder.Options);
        }
    }
}
