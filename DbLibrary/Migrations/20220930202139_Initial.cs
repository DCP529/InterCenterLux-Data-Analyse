using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbLibrary.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "product",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false),
                    size = table.Column<int>(type: "integer", nullable: false),
                    color = table.Column<string>(type: "text", nullable: false),
                    amount = table.Column<int>(type: "integer", nullable: false),
                    start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    sold = table.Column<int>(type: "integer", nullable: false),
                    left = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "product");
        }
    }
}
