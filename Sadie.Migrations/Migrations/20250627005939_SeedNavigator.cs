using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sadie.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class SeedNavigator : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "navigator_tabs",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "query" },
                    { 2, "official_view" },
                    { 3, "hotel_view" },
                    { 4, "myworld_view" },
                    { 5, "roomads_view" }
                });

            migrationBuilder.InsertData(
                table: "navigator_categories",
                columns: new[] { "id", "name", "code_name", "tab_id", "order_id" },
                values: new object[,]
                {
                    { 1, "Most Popular Rooms", "popular", 3, 0 },
                    { 2, "My Rooms", "my_rooms", 4, 0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "navigator_categories",
                keyColumn: "id",
                keyValues: new object[] { 1, 2 });

            migrationBuilder.DeleteData(
                table: "navigator_tabs",
                keyColumn: "id",
                keyValues: new object[] { 1, 2, 3, 4, 5 });
        }
    }
}
