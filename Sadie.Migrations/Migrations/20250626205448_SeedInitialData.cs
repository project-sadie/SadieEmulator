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
            migrationBuilder.Sql("INSERT INTO server_settings (player_welcome_message, fair_currency_rewards) VALUES ('Welcome (back) to Sadie [username], we''re running version [version]!', 1)");
            migrationBuilder.Sql("INSERT INTO server_player_constants (max_motto_length, min_sso_length, max_friendships) VALUES (35, 8, 20000)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("TRUNCATE TABLE server_settings;");
            migrationBuilder.Sql("TRUNCATE TABLE server_player_constants;");
        }
    }
}
