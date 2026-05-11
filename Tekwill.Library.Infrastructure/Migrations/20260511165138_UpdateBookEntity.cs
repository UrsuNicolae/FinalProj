using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tekwill.Library.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBookEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "books",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(2026, 5, 11, 16, 51, 37, 965, DateTimeKind.Utc).AddTicks(827));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "books");
        }
    }
}
