using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sadie.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class SeedConstants : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "server_settings",
                columns: new[] { "player_welcome_message", "fair_currency_rewards" },
                values: new object[,]
                {
                    { "Welcome (back) to Sadie [username], we're running version [version]!", 1 }
                });

            migrationBuilder.InsertData(
                table: "server_player_constants",
                columns: new[] { "max_motto_length", "min_sso_length", "max_friendships" },
                values: new object[,]
                {
                    { 35, 8, 20000 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "server_settings",
                keyColumns: new[] { "player_welcome_message", "fair_currency_rewards" },
                keyValues: new object[] { "Welcome (back) to Sadie [username], we're running version [version]!", 1 });

            migrationBuilder.DeleteData(
                table: "server_player_constants",
                keyColumns: new[] { "max_motto_length", "min_sso_length", "max_friendships" },
                keyValues: new object[] { 35, 8, 20000 });
        }
    }
}
