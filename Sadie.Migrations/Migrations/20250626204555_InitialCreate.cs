using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sadie.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "badges",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    code = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_badges", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "catalog_club_offers",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    duration_days = table.Column<int>(type: "int", nullable: false),
                    cost_credits = table.Column<int>(type: "int", nullable: false),
                    cost_points = table.Column<int>(type: "int", nullable: false),
                    cost_points_type = table.Column<int>(type: "int", nullable: false),
                    is_vip = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_catalog_club_offers", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "catalog_pages",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    caption = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    layout = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    role_id = table.Column<int>(type: "int", nullable: true),
                    catalog_page_id = table.Column<int>(type: "int", nullable: true),
                    order_id = table.Column<int>(type: "int", nullable: false),
                    icon_id = table.Column<int>(type: "int", nullable: false),
                    enabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    visible = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    images_json = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    texts_json = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_catalog_pages", x => x.id);
                    table.ForeignKey(
                        name: "fk_catalog_pages_catalog_pages_catalog_page_id",
                        column: x => x.catalog_page_id,
                        principalTable: "catalog_pages",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "furniture_items",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    asset_name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    type = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    asset_id = table.Column<int>(type: "int", nullable: false),
                    tile_span_x = table.Column<int>(type: "int", nullable: false),
                    tile_span_y = table.Column<int>(type: "int", nullable: false),
                    stack_height = table.Column<double>(type: "double", nullable: false),
                    can_stack = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    can_walk = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    can_sit = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    can_lay = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    can_recycle = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    can_trade = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    can_marketplace_sell = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    can_inventory_stack = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    can_gift = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    interaction_type = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    interaction_modes = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_furniture_items", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "hand_items",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_hand_items", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "navigator_tabs",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_navigator_tabs", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "permissions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_permissions", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "player_sso_tokens",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    player_id = table.Column<long>(type: "bigint", nullable: false),
                    token = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    expires_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    used_at = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_sso_tokens", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "players",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    username = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_players", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_roles", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "room_categories",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    caption = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_visible = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_room_categories", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "room_dimmer_presets",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    room_id = table.Column<long>(type: "bigint", nullable: false),
                    preset_id = table.Column<int>(type: "int", nullable: false),
                    background_only = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    color = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    intensity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_room_dimmer_presets", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "room_layouts",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    heightmap = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    door_x = table.Column<int>(type: "int", nullable: false),
                    door_y = table.Column<int>(type: "int", nullable: false),
                    door_direction = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_room_layouts", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "server_periodic_currency_rewards",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    type = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    amount = table.Column<int>(type: "int", nullable: false),
                    interval_seconds = table.Column<int>(type: "int", nullable: false),
                    skip_idle = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    skip_hotel_view = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_server_periodic_currency_rewards", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "server_player_constants",
                columns: table => new
                {
                    max_motto_length = table.Column<int>(type: "int", nullable: false),
                    min_sso_length = table.Column<int>(type: "int", nullable: false),
                    max_friendships = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "server_room_constants",
                columns: table => new
                {
                    max_chat_message_length = table.Column<int>(type: "int", nullable: false),
                    seconds_till_user_idle = table.Column<int>(type: "int", nullable: false),
                    max_name_length = table.Column<int>(type: "int", nullable: false),
                    max_description_length = table.Column<int>(type: "int", nullable: false),
                    max_tag_length = table.Column<int>(type: "int", nullable: false),
                    wired_max_furniture_selection = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "server_settings",
                columns: table => new
                {
                    player_welcome_message = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    fair_currency_rewards = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "subscriptions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_subscriptions", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "catalog_front_page_items",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    title = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    image = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    type_id = table.Column<int>(type: "int", nullable: false),
                    product_name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    catalog_page_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_catalog_front_page_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_catalog_front_page_items_catalog_pages_catalog_page_id",
                        column: x => x.catalog_page_id,
                        principalTable: "catalog_pages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "catalog_items",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cost_credits = table.Column<int>(type: "int", nullable: false),
                    cost_points = table.Column<int>(type: "int", nullable: false),
                    cost_points_type = table.Column<int>(type: "int", nullable: false),
                    requires_club_membership = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    meta_data = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    amount = table.Column<int>(type: "int", nullable: false),
                    sell_limit = table.Column<int>(type: "int", nullable: false),
                    catalog_page_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_catalog_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_catalog_items_catalog_pages_catalog_page_id",
                        column: x => x.catalog_page_id,
                        principalTable: "catalog_pages",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "furniture_item_hand_item",
                columns: table => new
                {
                    furniture_items_id = table.Column<int>(type: "int", nullable: false),
                    hand_items_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_furniture_item_hand_item", x => new { x.furniture_items_id, x.hand_items_id });
                    table.ForeignKey(
                        name: "fk_furniture_item_hand_item_furniture_items_furniture_items_id",
                        column: x => x.furniture_items_id,
                        principalTable: "furniture_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_furniture_item_hand_item_hand_item_hand_items_id",
                        column: x => x.hand_items_id,
                        principalTable: "hand_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "navigator_categories",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    code_name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    order_id = table.Column<int>(type: "int", nullable: false),
                    tab_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_navigator_categories", x => x.id);
                    table.ForeignKey(
                        name: "fk_navigator_categories_navigator_tabs_tab_id",
                        column: x => x.tab_id,
                        principalTable: "navigator_tabs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "banned_ip_addresses",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    creator_id = table.Column<long>(type: "bigint", nullable: false),
                    reason = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ip_address = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    expires_at = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_banned_ip_addresses", x => x.id);
                    table.ForeignKey(
                        name: "fk_banned_ip_addresses_players_creator_id",
                        column: x => x.creator_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "player_avatar_data",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    player_id = table.Column<long>(type: "bigint", nullable: false),
                    figure_code = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    motto = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    gender = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    chat_bubble_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_avatar_data", x => x.id);
                    table.ForeignKey(
                        name: "fk_player_avatar_data_players_player_id",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "player_badges",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    player_id = table.Column<long>(type: "bigint", nullable: false),
                    badge_id = table.Column<int>(type: "int", nullable: false),
                    slot = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_badges", x => x.id);
                    table.ForeignKey(
                        name: "fk_player_badges_badges_badge_id",
                        column: x => x.badge_id,
                        principalTable: "badges",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_player_badges_players_player_id",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "player_bans",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    creator_id = table.Column<long>(type: "bigint", nullable: false),
                    player_id = table.Column<long>(type: "bigint", nullable: false),
                    reason = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    expires_at = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_bans", x => x.id);
                    table.ForeignKey(
                        name: "fk_player_bans_players_creator_id",
                        column: x => x.creator_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_player_bans_players_player_id",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "player_bots",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    player_id = table.Column<long>(type: "bigint", nullable: false),
                    room_id = table.Column<int>(type: "int", nullable: true),
                    username = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    figure_code = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    motto = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    gender = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    chat_bubble_id = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_bots", x => x.id);
                    table.ForeignKey(
                        name: "fk_player_bots_players_player_id",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "player_data",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    player_id = table.Column<long>(type: "bigint", nullable: false),
                    home_room_id = table.Column<int>(type: "int", nullable: false),
                    credit_balance = table.Column<int>(type: "int", nullable: false),
                    pixel_balance = table.Column<int>(type: "int", nullable: false),
                    seasonal_balance = table.Column<int>(type: "int", nullable: false),
                    gotw_points = table.Column<int>(type: "int", nullable: false),
                    respect_points = table.Column<int>(type: "int", nullable: false),
                    respect_points_pet = table.Column<int>(type: "int", nullable: false),
                    achievement_score = table.Column<int>(type: "int", nullable: false),
                    allow_friend_requests = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_online = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    last_online = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_data", x => x.id);
                    table.ForeignKey(
                        name: "fk_player_data_players_player_id",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "player_friendships",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    origin_player_id = table.Column<long>(type: "bigint", nullable: false),
                    target_player_id = table.Column<long>(type: "bigint", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_friendships", x => x.id);
                    table.ForeignKey(
                        name: "fk_player_friendships_players_origin_player_id",
                        column: x => x.origin_player_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_player_friendships_players_target_player_id",
                        column: x => x.target_player_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "player_furniture_items",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    player_id = table.Column<long>(type: "bigint", nullable: false),
                    furniture_item_id = table.Column<int>(type: "int", nullable: false),
                    limited_data = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    meta_data = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_furniture_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_player_furniture_items_furniture_items_furniture_item_id",
                        column: x => x.furniture_item_id,
                        principalTable: "furniture_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_player_furniture_items_players_player_id",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "player_game_settings",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    player_id = table.Column<long>(type: "bigint", nullable: false),
                    system_volume = table.Column<int>(type: "int", nullable: false),
                    furniture_volume = table.Column<int>(type: "int", nullable: false),
                    trax_volume = table.Column<int>(type: "int", nullable: false),
                    prefer_old_chat = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    block_room_invites = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    block_camera_follow = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ui_flags = table.Column<int>(type: "int", nullable: false),
                    show_notifications = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_game_settings", x => x.id);
                    table.ForeignKey(
                        name: "fk_player_game_settings_players_player_id",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "player_ignores",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    player_id = table.Column<long>(type: "bigint", nullable: false),
                    target_player_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_ignores", x => x.id);
                    table.ForeignKey(
                        name: "fk_player_ignores_players_player_id",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "player_messages",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    origin_player_id = table.Column<long>(type: "bigint", nullable: false),
                    target_player_id = table.Column<long>(type: "bigint", nullable: false),
                    message = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_messages", x => x.id);
                    table.ForeignKey(
                        name: "fk_player_messages_players_origin_player_id",
                        column: x => x.origin_player_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_player_messages_players_target_player_id",
                        column: x => x.target_player_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "player_navigator_settings",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    player_id = table.Column<long>(type: "bigint", nullable: false),
                    window_x = table.Column<int>(type: "int", nullable: false),
                    window_y = table.Column<int>(type: "int", nullable: false),
                    window_width = table.Column<int>(type: "int", nullable: false),
                    window_height = table.Column<int>(type: "int", nullable: false),
                    open_searches = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_navigator_settings", x => x.id);
                    table.ForeignKey(
                        name: "fk_player_navigator_settings_players_player_id",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "player_relationships",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    origin_player_id = table.Column<long>(type: "bigint", nullable: false),
                    target_player_id = table.Column<long>(type: "bigint", nullable: false),
                    type_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_relationships", x => x.id);
                    table.ForeignKey(
                        name: "fk_player_relationships_players_origin_player_id",
                        column: x => x.origin_player_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_player_relationships_players_target_player_id",
                        column: x => x.target_player_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "player_respects",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    origin_player_id = table.Column<long>(type: "bigint", nullable: false),
                    target_player_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_respects", x => x.id);
                    table.ForeignKey(
                        name: "fk_player_respects_players_origin_player_id",
                        column: x => x.origin_player_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_player_respects_players_target_player_id",
                        column: x => x.target_player_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "player_room_visits",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    player_id = table.Column<long>(type: "bigint", nullable: false),
                    room_id = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_room_visits", x => x.id);
                    table.ForeignKey(
                        name: "fk_player_room_visits_players_player_id",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "player_saved_searches",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    search = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    filter = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    player_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_saved_searches", x => x.id);
                    table.ForeignKey(
                        name: "fk_player_saved_searches_players_player_id",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "player_tags",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    player_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_tags", x => x.id);
                    table.ForeignKey(
                        name: "fk_player_tags_players_player_id",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "player_wardrobe_items",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    slot_id = table.Column<int>(type: "int", nullable: false),
                    figure_code = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    gender = table.Column<int>(type: "int", nullable: false),
                    player_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_wardrobe_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_player_wardrobe_items_players_player_id",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "server_periodic_currency_reward_logs",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    player_id = table.Column<long>(type: "bigint", nullable: false),
                    type = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    amount = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_server_periodic_currency_reward_logs", x => x.id);
                    table.ForeignKey(
                        name: "fk_server_periodic_currency_reward_logs_players_player_id",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "player_role",
                columns: table => new
                {
                    role_id = table.Column<int>(type: "int", nullable: false),
                    player_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_role", x => new { x.role_id, x.player_id });
                    table.ForeignKey(
                        name: "fk_player_role_players_player_id",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_player_role_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "roles_permissions",
                columns: table => new
                {
                    permission_id = table.Column<int>(type: "int", nullable: false),
                    role_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_roles_permissions", x => new { x.permission_id, x.role_id });
                    table.ForeignKey(
                        name: "fk_roles_permissions_permissions_permission_id",
                        column: x => x.permission_id,
                        principalTable: "permissions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_roles_permissions_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "rooms",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    layout_id = table.Column<int>(type: "int", nullable: false),
                    owner_id = table.Column<long>(type: "bigint", nullable: false),
                    max_users_allowed = table.Column<int>(type: "int", nullable: false),
                    description = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_muted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_rooms", x => x.id);
                    table.ForeignKey(
                        name: "fk_rooms_players_owner_id",
                        column: x => x.owner_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_rooms_room_layouts_layout_id",
                        column: x => x.layout_id,
                        principalTable: "room_layouts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "player_subscriptions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    player_id = table.Column<long>(type: "bigint", nullable: false),
                    subscription_id = table.Column<int>(type: "int", nullable: false),
                    expires_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_subscriptions", x => x.id);
                    table.ForeignKey(
                        name: "fk_player_subscriptions_players_player_id",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_player_subscriptions_subscriptions_subscription_id",
                        column: x => x.subscription_id,
                        principalTable: "subscriptions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "catalog_item_furniture_item",
                columns: table => new
                {
                    catalog_items_id = table.Column<int>(type: "int", nullable: false),
                    furniture_items_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_catalog_item_furniture_item", x => new { x.catalog_items_id, x.furniture_items_id });
                    table.ForeignKey(
                        name: "fk_catalog_item_furniture_item_catalog_items_catalog_items_id",
                        column: x => x.catalog_items_id,
                        principalTable: "catalog_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_catalog_item_furniture_item_furniture_items_furniture_items_",
                        column: x => x.furniture_items_id,
                        principalTable: "furniture_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "player_furniture_item_links",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    parent_id = table.Column<int>(type: "int", nullable: false),
                    child_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_furniture_item_links", x => x.id);
                    table.ForeignKey(
                        name: "fk_player_furniture_item_links_player_furniture_items_child_id",
                        column: x => x.child_id,
                        principalTable: "player_furniture_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_player_furniture_item_links_player_furniture_items_parent_id",
                        column: x => x.parent_id,
                        principalTable: "player_furniture_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "groups",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    player_id = table.Column<long>(type: "bigint", nullable: false),
                    name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    room_id = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_groups", x => x.id);
                    table.ForeignKey(
                        name: "fk_groups_rooms_room_id",
                        column: x => x.room_id,
                        principalTable: "rooms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "player_furniture_item_placement_data",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    player_furniture_item_id = table.Column<int>(type: "int", nullable: false),
                    room_id = table.Column<int>(type: "int", nullable: false),
                    position_x = table.Column<int>(type: "int", nullable: false),
                    position_y = table.Column<int>(type: "int", nullable: false),
                    position_z = table.Column<double>(type: "double", nullable: false),
                    wall_position = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    direction = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_furniture_item_placement_data", x => x.id);
                    table.ForeignKey(
                        name: "fk_player_furniture_item_placement_data_player_furniture_items_",
                        column: x => x.player_furniture_item_id,
                        principalTable: "player_furniture_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_player_furniture_item_placement_data_rooms_room_id",
                        column: x => x.room_id,
                        principalTable: "rooms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "player_room_bans",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    player_id = table.Column<long>(type: "bigint", nullable: false),
                    room_id = table.Column<int>(type: "int", nullable: false),
                    expires_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_room_bans", x => x.id);
                    table.ForeignKey(
                        name: "fk_player_room_bans_players_player_id",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_player_room_bans_rooms_room_id",
                        column: x => x.room_id,
                        principalTable: "rooms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "player_room_likes",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    player_id = table.Column<long>(type: "bigint", nullable: false),
                    room_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_room_likes", x => x.id);
                    table.ForeignKey(
                        name: "fk_player_room_likes_players_player_id",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_player_room_likes_rooms_room_id",
                        column: x => x.room_id,
                        principalTable: "rooms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "room_chat_messages",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    room_id = table.Column<int>(type: "int", nullable: false),
                    player_id = table.Column<long>(type: "bigint", nullable: false),
                    message = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    chat_bubble_id = table.Column<int>(type: "int", nullable: false),
                    emotion_id = table.Column<int>(type: "int", nullable: false),
                    type_id = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_room_chat_messages", x => x.id);
                    table.ForeignKey(
                        name: "fk_room_chat_messages_players_player_id",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_room_chat_messages_rooms_room_id",
                        column: x => x.room_id,
                        principalTable: "rooms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "room_chat_settings",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    room_id = table.Column<int>(type: "int", nullable: false),
                    chat_type = table.Column<int>(type: "int", nullable: false),
                    chat_weight = table.Column<int>(type: "int", nullable: false),
                    chat_speed = table.Column<int>(type: "int", nullable: false),
                    chat_distance = table.Column<int>(type: "int", nullable: false),
                    chat_protection = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_room_chat_settings", x => x.id);
                    table.ForeignKey(
                        name: "fk_room_chat_settings_rooms_room_id",
                        column: x => x.room_id,
                        principalTable: "rooms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "room_dimmer_settings",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    room_id = table.Column<int>(type: "int", nullable: false),
                    enabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    preset_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_room_dimmer_settings", x => x.id);
                    table.ForeignKey(
                        name: "fk_room_dimmer_settings_rooms_room_id",
                        column: x => x.room_id,
                        principalTable: "rooms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "room_paint_settings",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    room_id = table.Column<int>(type: "int", nullable: false),
                    floor_paint = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    wall_paint = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    landscape_paint = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_room_paint_settings", x => x.id);
                    table.ForeignKey(
                        name: "fk_room_paint_settings_rooms_room_id",
                        column: x => x.room_id,
                        principalTable: "rooms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "room_player_rights",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    room_id = table.Column<int>(type: "int", nullable: false),
                    player_id = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_room_player_rights", x => x.id);
                    table.ForeignKey(
                        name: "fk_room_player_rights_rooms_room_id",
                        column: x => x.room_id,
                        principalTable: "rooms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "room_settings",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    room_id = table.Column<int>(type: "int", nullable: false),
                    walk_diagonal = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    access_type = table.Column<int>(type: "int", nullable: false),
                    password = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    who_can_mute = table.Column<int>(type: "int", nullable: false),
                    who_can_kick = table.Column<int>(type: "int", nullable: false),
                    who_can_ban = table.Column<int>(type: "int", nullable: false),
                    allow_pets = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    can_pets_eat = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    hide_walls = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    wall_thickness = table.Column<int>(type: "int", nullable: false),
                    floor_thickness = table.Column<int>(type: "int", nullable: false),
                    can_users_overlap = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    trade_option = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_room_settings", x => x.id);
                    table.ForeignKey(
                        name: "fk_room_settings_rooms_room_id",
                        column: x => x.room_id,
                        principalTable: "rooms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "room_tags",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    room_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_room_tags", x => x.id);
                    table.ForeignKey(
                        name: "fk_room_tags_rooms_room_id",
                        column: x => x.room_id,
                        principalTable: "rooms",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "group_player",
                columns: table => new
                {
                    group_id = table.Column<int>(type: "int", nullable: false),
                    player_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_group_player", x => new { x.group_id, x.player_id });
                    table.ForeignKey(
                        name: "fk_group_player_groups_group_id",
                        column: x => x.group_id,
                        principalTable: "groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_group_player_players_player_id",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "player_furniture_item_wired_data",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    player_furniture_item_placement_data_id = table.Column<int>(type: "int", nullable: false),
                    message = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    delay = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_furniture_item_wired_data", x => x.id);
                    table.ForeignKey(
                        name: "fk_player_furniture_item_wired_data_player_furniture_item_place",
                        column: x => x.player_furniture_item_placement_data_id,
                        principalTable: "player_furniture_item_placement_data",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "player_furniture_item_wired_data_items",
                columns: table => new
                {
                    player_furniture_item_placement_data_id = table.Column<int>(type: "int", nullable: false),
                    player_furniture_item_wired_data_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_furniture_item_wired_data_items", x => new { x.player_furniture_item_placement_data_id, x.player_furniture_item_wired_data_id });
                    table.ForeignKey(
                        name: "fk_player_furniture_item_wired_data_items_player_furniture_item",
                        column: x => x.player_furniture_item_placement_data_id,
                        principalTable: "player_furniture_item_placement_data",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_player_furniture_item_wired_data_items_player_furniture_item1",
                        column: x => x.player_furniture_item_wired_data_id,
                        principalTable: "player_furniture_item_wired_data",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "ix_banned_ip_addresses_creator_id",
                table: "banned_ip_addresses",
                column: "creator_id");

            migrationBuilder.CreateIndex(
                name: "ix_catalog_front_page_items_catalog_page_id",
                table: "catalog_front_page_items",
                column: "catalog_page_id");

            migrationBuilder.CreateIndex(
                name: "ix_catalog_item_furniture_item_furniture_items_id",
                table: "catalog_item_furniture_item",
                column: "furniture_items_id");

            migrationBuilder.CreateIndex(
                name: "ix_catalog_items_catalog_page_id",
                table: "catalog_items",
                column: "catalog_page_id");

            migrationBuilder.CreateIndex(
                name: "ix_catalog_pages_catalog_page_id",
                table: "catalog_pages",
                column: "catalog_page_id");

            migrationBuilder.CreateIndex(
                name: "ix_furniture_item_hand_item_hand_items_id",
                table: "furniture_item_hand_item",
                column: "hand_items_id");

            migrationBuilder.CreateIndex(
                name: "ix_group_player_player_id",
                table: "group_player",
                column: "player_id");

            migrationBuilder.CreateIndex(
                name: "ix_groups_room_id",
                table: "groups",
                column: "room_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_navigator_categories_tab_id",
                table: "navigator_categories",
                column: "tab_id");

            migrationBuilder.CreateIndex(
                name: "ix_player_avatar_data_player_id",
                table: "player_avatar_data",
                column: "player_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_player_badges_badge_id",
                table: "player_badges",
                column: "badge_id");

            migrationBuilder.CreateIndex(
                name: "ix_player_badges_player_id",
                table: "player_badges",
                column: "player_id");

            migrationBuilder.CreateIndex(
                name: "ix_player_bans_creator_id",
                table: "player_bans",
                column: "creator_id");

            migrationBuilder.CreateIndex(
                name: "ix_player_bans_player_id",
                table: "player_bans",
                column: "player_id");

            migrationBuilder.CreateIndex(
                name: "ix_player_bots_player_id",
                table: "player_bots",
                column: "player_id");

            migrationBuilder.CreateIndex(
                name: "ix_player_data_player_id",
                table: "player_data",
                column: "player_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_player_friendships_origin_player_id",
                table: "player_friendships",
                column: "origin_player_id");

            migrationBuilder.CreateIndex(
                name: "ix_player_friendships_target_player_id",
                table: "player_friendships",
                column: "target_player_id");

            migrationBuilder.CreateIndex(
                name: "ix_player_furniture_item_links_child_id",
                table: "player_furniture_item_links",
                column: "child_id");

            migrationBuilder.CreateIndex(
                name: "ix_player_furniture_item_links_parent_id",
                table: "player_furniture_item_links",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "ix_player_furniture_item_placement_data_player_furniture_item_id",
                table: "player_furniture_item_placement_data",
                column: "player_furniture_item_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_player_furniture_item_placement_data_room_id",
                table: "player_furniture_item_placement_data",
                column: "room_id");

            migrationBuilder.CreateIndex(
                name: "ix_player_furniture_item_wired_data_player_furniture_item_place",
                table: "player_furniture_item_wired_data",
                column: "player_furniture_item_placement_data_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_player_furniture_item_wired_data_items_player_furniture_item",
                table: "player_furniture_item_wired_data_items",
                column: "player_furniture_item_wired_data_id");

            migrationBuilder.CreateIndex(
                name: "ix_player_furniture_items_furniture_item_id",
                table: "player_furniture_items",
                column: "furniture_item_id");

            migrationBuilder.CreateIndex(
                name: "ix_player_furniture_items_player_id",
                table: "player_furniture_items",
                column: "player_id");

            migrationBuilder.CreateIndex(
                name: "ix_player_game_settings_player_id",
                table: "player_game_settings",
                column: "player_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_player_ignores_player_id",
                table: "player_ignores",
                column: "player_id");

            migrationBuilder.CreateIndex(
                name: "ix_player_messages_origin_player_id",
                table: "player_messages",
                column: "origin_player_id");

            migrationBuilder.CreateIndex(
                name: "ix_player_messages_target_player_id",
                table: "player_messages",
                column: "target_player_id");

            migrationBuilder.CreateIndex(
                name: "ix_player_navigator_settings_player_id",
                table: "player_navigator_settings",
                column: "player_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_player_relationships_origin_player_id",
                table: "player_relationships",
                column: "origin_player_id");

            migrationBuilder.CreateIndex(
                name: "ix_player_relationships_target_player_id",
                table: "player_relationships",
                column: "target_player_id");

            migrationBuilder.CreateIndex(
                name: "ix_player_respects_origin_player_id",
                table: "player_respects",
                column: "origin_player_id");

            migrationBuilder.CreateIndex(
                name: "ix_player_respects_target_player_id",
                table: "player_respects",
                column: "target_player_id");

            migrationBuilder.CreateIndex(
                name: "ix_player_role_player_id",
                table: "player_role",
                column: "player_id");

            migrationBuilder.CreateIndex(
                name: "ix_player_room_bans_player_id",
                table: "player_room_bans",
                column: "player_id");

            migrationBuilder.CreateIndex(
                name: "ix_player_room_bans_room_id",
                table: "player_room_bans",
                column: "room_id");

            migrationBuilder.CreateIndex(
                name: "ix_player_room_likes_player_id",
                table: "player_room_likes",
                column: "player_id");

            migrationBuilder.CreateIndex(
                name: "ix_player_room_likes_room_id",
                table: "player_room_likes",
                column: "room_id");

            migrationBuilder.CreateIndex(
                name: "ix_player_room_visits_player_id",
                table: "player_room_visits",
                column: "player_id");

            migrationBuilder.CreateIndex(
                name: "ix_player_saved_searches_player_id",
                table: "player_saved_searches",
                column: "player_id");

            migrationBuilder.CreateIndex(
                name: "ix_player_subscriptions_player_id",
                table: "player_subscriptions",
                column: "player_id");

            migrationBuilder.CreateIndex(
                name: "ix_player_subscriptions_subscription_id",
                table: "player_subscriptions",
                column: "subscription_id");

            migrationBuilder.CreateIndex(
                name: "ix_player_tags_player_id",
                table: "player_tags",
                column: "player_id");

            migrationBuilder.CreateIndex(
                name: "ix_player_wardrobe_items_player_id",
                table: "player_wardrobe_items",
                column: "player_id");

            migrationBuilder.CreateIndex(
                name: "ix_roles_permissions_role_id",
                table: "roles_permissions",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ix_room_chat_messages_player_id",
                table: "room_chat_messages",
                column: "player_id");

            migrationBuilder.CreateIndex(
                name: "ix_room_chat_messages_room_id",
                table: "room_chat_messages",
                column: "room_id");

            migrationBuilder.CreateIndex(
                name: "ix_room_chat_settings_room_id",
                table: "room_chat_settings",
                column: "room_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_room_dimmer_settings_room_id",
                table: "room_dimmer_settings",
                column: "room_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_room_paint_settings_room_id",
                table: "room_paint_settings",
                column: "room_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_room_player_rights_room_id",
                table: "room_player_rights",
                column: "room_id");

            migrationBuilder.CreateIndex(
                name: "ix_room_settings_room_id",
                table: "room_settings",
                column: "room_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_room_tags_room_id",
                table: "room_tags",
                column: "room_id");

            migrationBuilder.CreateIndex(
                name: "ix_rooms_layout_id",
                table: "rooms",
                column: "layout_id");

            migrationBuilder.CreateIndex(
                name: "ix_rooms_owner_id",
                table: "rooms",
                column: "owner_id");

            migrationBuilder.CreateIndex(
                name: "ix_server_periodic_currency_reward_logs_player_id",
                table: "server_periodic_currency_reward_logs",
                column: "player_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "banned_ip_addresses");

            migrationBuilder.DropTable(
                name: "catalog_club_offers");

            migrationBuilder.DropTable(
                name: "catalog_front_page_items");

            migrationBuilder.DropTable(
                name: "catalog_item_furniture_item");

            migrationBuilder.DropTable(
                name: "furniture_item_hand_item");

            migrationBuilder.DropTable(
                name: "group_player");

            migrationBuilder.DropTable(
                name: "navigator_categories");

            migrationBuilder.DropTable(
                name: "player_avatar_data");

            migrationBuilder.DropTable(
                name: "player_badges");

            migrationBuilder.DropTable(
                name: "player_bans");

            migrationBuilder.DropTable(
                name: "player_bots");

            migrationBuilder.DropTable(
                name: "player_data");

            migrationBuilder.DropTable(
                name: "player_friendships");

            migrationBuilder.DropTable(
                name: "player_furniture_item_links");

            migrationBuilder.DropTable(
                name: "player_furniture_item_wired_data_items");

            migrationBuilder.DropTable(
                name: "player_game_settings");

            migrationBuilder.DropTable(
                name: "player_ignores");

            migrationBuilder.DropTable(
                name: "player_messages");

            migrationBuilder.DropTable(
                name: "player_navigator_settings");

            migrationBuilder.DropTable(
                name: "player_relationships");

            migrationBuilder.DropTable(
                name: "player_respects");

            migrationBuilder.DropTable(
                name: "player_role");

            migrationBuilder.DropTable(
                name: "player_room_bans");

            migrationBuilder.DropTable(
                name: "player_room_likes");

            migrationBuilder.DropTable(
                name: "player_room_visits");

            migrationBuilder.DropTable(
                name: "player_saved_searches");

            migrationBuilder.DropTable(
                name: "player_sso_tokens");

            migrationBuilder.DropTable(
                name: "player_subscriptions");

            migrationBuilder.DropTable(
                name: "player_tags");

            migrationBuilder.DropTable(
                name: "player_wardrobe_items");

            migrationBuilder.DropTable(
                name: "roles_permissions");

            migrationBuilder.DropTable(
                name: "room_categories");

            migrationBuilder.DropTable(
                name: "room_chat_messages");

            migrationBuilder.DropTable(
                name: "room_chat_settings");

            migrationBuilder.DropTable(
                name: "room_dimmer_presets");

            migrationBuilder.DropTable(
                name: "room_dimmer_settings");

            migrationBuilder.DropTable(
                name: "room_paint_settings");

            migrationBuilder.DropTable(
                name: "room_player_rights");

            migrationBuilder.DropTable(
                name: "room_settings");

            migrationBuilder.DropTable(
                name: "room_tags");

            migrationBuilder.DropTable(
                name: "server_periodic_currency_reward_logs");

            migrationBuilder.DropTable(
                name: "server_periodic_currency_rewards");

            migrationBuilder.DropTable(
                name: "server_player_constants");

            migrationBuilder.DropTable(
                name: "server_room_constants");

            migrationBuilder.DropTable(
                name: "server_settings");

            migrationBuilder.DropTable(
                name: "catalog_items");

            migrationBuilder.DropTable(
                name: "hand_items");

            migrationBuilder.DropTable(
                name: "groups");

            migrationBuilder.DropTable(
                name: "navigator_tabs");

            migrationBuilder.DropTable(
                name: "badges");

            migrationBuilder.DropTable(
                name: "player_furniture_item_wired_data");

            migrationBuilder.DropTable(
                name: "subscriptions");

            migrationBuilder.DropTable(
                name: "permissions");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "catalog_pages");

            migrationBuilder.DropTable(
                name: "player_furniture_item_placement_data");

            migrationBuilder.DropTable(
                name: "player_furniture_items");

            migrationBuilder.DropTable(
                name: "rooms");

            migrationBuilder.DropTable(
                name: "furniture_items");

            migrationBuilder.DropTable(
                name: "players");

            migrationBuilder.DropTable(
                name: "room_layouts");
        }
    }
}
