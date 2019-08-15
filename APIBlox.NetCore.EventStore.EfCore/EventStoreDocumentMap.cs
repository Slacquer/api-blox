using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace APIBlox.NetCore
{
    internal class EventStoreDocumentMap
    {
        private readonly EntityTypeBuilder<DocEx> _builder;

        public EventStoreDocumentMap(EntityTypeBuilder<DocEx> builder)
        {
            _builder = builder;
        }

        public void Map()
        {
            _builder.ToTable("EventStoreDocuments");

            _builder.HasKey(p => p.Id);

            _builder.HasIndex(p => p.StreamId);

            //_builder.Ignore(p => p.Data);
            _builder.Property(p => p.Data);

            _builder.Property(p => p.DataType).HasMaxLength(1024);
            _builder.Property(p => p.DocumentType).HasMaxLength(255);

            // _builder.Property(p => p.SortOrder);
            _builder.Property(p => p.StreamId).IsRequired().HasMaxLength(255);
            _builder.Property(p => p.TimeStamp).IsRequired();
            _builder.Property(p => p.Version).IsRequired();
        }
    }
}
