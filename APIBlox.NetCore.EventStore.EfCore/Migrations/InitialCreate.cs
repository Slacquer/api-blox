using Microsoft.EntityFrameworkCore.Migrations;

namespace APIBlox.NetCore.Migrations
{
    internal partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "EventStoreDocuments",
                table => new
                {
                    Id = table.Column<string>(),
                    StreamId = table.Column<string>(maxLength: 255 ),
                    DocumentType = table.Column<int>(maxLength: 255),
                    DataType = table.Column<string>(maxLength: 1024, nullable: true),
                    DataEx = table.Column<string>(nullable: true),
                    Version = table.Column<long>(),
                    TimeStamp = table.Column<long>()
                },
                constraints: table => { table.PrimaryKey("PK_EventStoreDocuments", x => x.Id); }
            );

            migrationBuilder.CreateIndex(
                "IX_EventStoreDocuments_StreamId",
                "EventStoreDocuments",
                "StreamId"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "EventStoreDocuments"
            );
        }
    }
}
