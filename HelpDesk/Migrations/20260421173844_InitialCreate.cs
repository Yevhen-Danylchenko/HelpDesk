using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HelpDesk.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    AuthorId = table.Column<string>(type: "TEXT", nullable: false),
                    AuthorName = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Tickets",
                columns: new[] { "Id", "AuthorId", "AuthorName", "CreatedAt", "Description", "Status", "Title" },
                values: new object[,]
                {
                    { 1, "demo", "ivan", new DateTime(2026, 3, 15, 10, 0, 0, 0, DateTimeKind.Unspecified), "Принтер на 3 поверсі не друкує", "Новий", "Не працює принтер" },
                    { 2, "demo", "ivan", new DateTime(2026, 3, 15, 11, 0, 0, 0, DateTimeKind.Unspecified), "Не можу підключитися до корпоративного VPN з дому", "В роботі", "Потрібен доступ до VPN" },
                    { 3, "demo", "ivan", new DateTime(2026, 3, 15, 12, 30, 0, 0, DateTimeKind.Unspecified), "Ліцензія Microsoft Office закінчилась на моєму ПК", "Вирішено", "Оновити ліцензію Office" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tickets");
        }
    }
}
