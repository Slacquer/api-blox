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

        public DbSet<EventStoreDocument> Documents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var eventStoreDocumentMap = new EventStoreDocumentMap(modelBuilder.Entity<EventStoreDocument>());

            eventStoreDocumentMap.Map();
        }
    }
}
