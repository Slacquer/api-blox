﻿// <auto-generated />

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace APIBlox.NetCore.Migrations
{
    [DbContext(typeof(EventStoreDbContext))]
    internal class EventStoreDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.0-rtm-35687")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("APIBlox.NetCore.Documents.EventStoreDocument",
                b =>
                {
                    b.Property<string>("Id").ValueGeneratedOnAdd();
                    b.Property<string>("DataEx");
                    b.Property<string>("DataType").HasMaxLength(1024);
                    b.Property<int>("DocumentType").HasMaxLength(255);
                    b.Property<string>("StreamId").IsRequired().HasMaxLength(255);
                    b.Property<long>("TimeStamp");
                    b.Property<long>("Version");
                    b.HasKey("Id");
                    b.HasIndex("StreamId");
                    b.ToTable("EventStoreDocuments");
                }
            );
#pragma warning restore 612, 618
        }
    }
}
