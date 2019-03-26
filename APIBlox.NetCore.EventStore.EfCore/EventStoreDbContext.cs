using APIBlox.NetCore.Documents;
using Microsoft.EntityFrameworkCore;

namespace APIBlox.NetCore
{
    internal class EventStoreDbContext : DbContext
    {
        public EventStoreDbContext(DbContextOptions<EventStoreDbContext> options)
            : base(options)
        {
        }

        public DbSet<EventStoreDocumentEx> Documents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var eventStoreDocumentMap = new EventStoreDocumentMap(modelBuilder.Entity<EventStoreDocumentEx>());

            eventStoreDocumentMap.Map();
        }
    }
}
