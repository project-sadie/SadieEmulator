using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MySqlConnector;
using Sadie.Database.Models;
using Sadie.Database.Models.Catalog;
using Sadie.Database.Models.Catalog.FrontPage;
using Sadie.Database.Models.Catalog.Items;
using Sadie.Database.Models.Catalog.Pages;
using Sadie.Database.Models.Constants;
using Sadie.Database.Models.Furniture;
using Sadie.Database.Models.Navigator;
using Sadie.Database.Models.Players;
using Sadie.Database.Models.Rooms;
using Sadie.Database.Models.Rooms.Chat;
using Sadie.Database.Models.Rooms.Furniture;
using Sadie.Database.Models.Rooms.Rights;
using Sadie.Options.Models;
using Sadie.Shared.Helpers;
using Sadie.Shared.Unsorted;
using Sadie.Shared.Unsorted.Game.Avatar;

namespace Sadie.Database;

public class SadieContext(IOptions<DatabaseOptions> options) : DbContext
{
    public DbSet<NavigatorCategory> NavigatorCategories { get; init; }
    public DbSet<NavigatorTab> NavigatorTabs { get; init; }
    public DbSet<FurnitureItem> FurnitureItems { get; init; }
    public DbSet<CatalogItem> CatalogItems { get; init; }
    public DbSet<CatalogPage> CatalogPages { get; init; }
    public DbSet<CatalogFrontPageItem> CatalogFrontPageItems { get; init; }
    public DbSet<RoomCategory> RoomCategories { get; init; }
    public DbSet<RoomChatMessage> RoomChatMessages { get; init; }
    public DbSet<RoomFurnitureItem> RoomFurnitureItems { get; init; }
    public DbSet<RoomPlayerRight> RoomPlayerRights { get; init; }
    public DbSet<RoomSettings> RoomSettings { get; init; }
    public DbSet<RoomLayout> RoomLayouts { get; init; }
    public DbSet<Room> Rooms { get; init; }
    public DbSet<Player> Players { get; init; }
    public DbSet<PlayerData> PlayerData { get; init; }
    public DbSet<PlayerFurnitureItem> PlayerFurnitureItems { get; init; }
    public DbSet<PlayerBadge> PlayerBadges { get; init; }
    public DbSet<Badge> Badges { get; init; }
    public DbSet<CatalogClubOffer> CatalogClubOffers { get; init; }
    public DbSet<ServerPlayerConstants> ServerPlayerConstants { get; init; }
    public DbSet<ServerRoomConstants> ServerRoomConstants { get; init; }
    public DbSet<ServerSettings> ServerSettings { get; init; }
    public DbSet<PlayerSsoToken> PlayerSsoToken { get; init; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var databaseSettings = options.Value;
        var stringBuilder = new MySqlConnectionStringBuilder
        {
            UserID = databaseSettings.Username,
            Server = databaseSettings.Host,
            Database = databaseSettings.Database,
            Port = databaseSettings.Port,
            Password = databaseSettings.Password
        };

        var connectionString = stringBuilder.ToString();

        optionsBuilder.UseMySql(connectionString, MySqlServerVersion.LatestSupportedServerVersion, mySqlOptions =>
        {
            mySqlOptions.EnableRetryOnFailure(
               maxRetryCount: 10,
               maxRetryDelay: TimeSpan.FromSeconds(30),
               errorNumbersToAdd: null);
        });
        optionsBuilder.UseSnakeCaseNamingConvention();

        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NavigatorCategory>()
            .HasOne<NavigatorTab>(e => e.Tab)
            .WithMany(g => g.Categories)
            .HasForeignKey(s => s.TabId);

        modelBuilder.Entity<FurnitureItem>()
            .Property(e => e.Type)
            .HasConversion(
                v => v.ToString(),
                v => EnumHelpers.GetEnumValueFromDescription<FurnitureItemType>(v));

        modelBuilder.Entity<CatalogItem>()
            .HasMany(c => c.FurnitureItems)
            .WithMany(x => x.CatalogItems);

        modelBuilder.Entity<Room>()
            .HasOne<RoomPaintSettings>(e => e.PaintSettings)
            .WithOne(x => x.Room)
            .HasForeignKey<RoomPaintSettings>(x => x.RoomId);

        modelBuilder.Entity<Player>().ToTable("players");
        modelBuilder.Entity<PlayerSsoToken>().ToTable("player_sso_tokens");
        modelBuilder.Entity<PlayerRoomLike>().ToTable("player_room_likes");
        modelBuilder.Entity<RoomTag>().ToTable("room_tags");

        modelBuilder.Entity<RoomLayout>()
            .Property(x => x.HeightMap)
            .HasColumnName("heightmap");

        modelBuilder.Entity<PlayerTag>().ToTable("player_tags");
        modelBuilder.Entity<PlayerRelationship>().ToTable("player_relationships");
        modelBuilder.Entity<PlayerFurnitureItem>().ToTable("player_furniture_items");
        modelBuilder.Entity<PlayerWardrobeItem>().ToTable("player_wardrobe_items");
        modelBuilder.Entity<Permission>().ToTable("permissions");
        modelBuilder.Entity<PlayerSubscription>().ToTable("player_subscriptions");
        modelBuilder.Entity<PlayerRespect>().ToTable("player_respects");
        modelBuilder.Entity<PlayerSavedSearch>().ToTable("player_saved_searches");
        modelBuilder.Entity<PlayerFriendship>().ToTable("player_friendships");
        modelBuilder.Entity<PlayerMessage>().ToTable("player_messages");
        modelBuilder.Entity<Role>().ToTable("roles");

        modelBuilder.Entity<PlayerAvatarData>()
            .Property(e => e.Gender)
            .HasConversion(
                v => v.ToString(),
                v => EnumHelpers.GetEnumValueFromDescription<AvatarGender>(v));

        modelBuilder.Entity<Subscription>().ToTable("subscriptions");

        modelBuilder.Entity<Player>()
            .HasMany(r => r.Roles)
            .WithMany(p => p.Players)
            .UsingEntity("player_role",
                l => l.HasOne(typeof(Role)).WithMany().HasForeignKey("role_id").HasPrincipalKey(nameof(Role.Id)),
                r => r.HasOne(typeof(Player)).WithMany().HasForeignKey("player_id").HasPrincipalKey(nameof(Player.Id)),
                j => j.HasKey("role_id", "player_id"));

        modelBuilder.Entity<Role>()
            .HasMany(r => r.Permissions)
            .WithMany(p => p.Roles)
            .UsingEntity("roles_permissions",
                l => l.HasOne(typeof(Permission)).WithMany().HasForeignKey("permission_id").HasPrincipalKey(nameof(Permission.Id)),
                r => r.HasOne(typeof(Role)).WithMany().HasForeignKey("role_id").HasPrincipalKey(nameof(Role.Id)),
                j => j.HasKey("permission_id", "role_id"));

        modelBuilder.Entity<ServerPlayerConstants>(builder => builder.HasNoKey());
        modelBuilder.Entity<ServerRoomConstants>(builder => builder.HasNoKey());
        modelBuilder.Entity<ServerSettings>(builder => builder.HasNoKey());
    }
}