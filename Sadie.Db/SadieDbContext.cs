using Microsoft.EntityFrameworkCore;
using Sadie.Core.Enums.Game.Furniture;
using Sadie.Core.Enums.Game.Players;
using Sadie.Core.Enums.Game.Rooms;
using Sadie.Core.Enums.Miscellaneous;
using Sadie.Core.Shared.Helpers;
using Sadie.Db.Models;
using Sadie.Db.Models.Catalog;
using Sadie.Db.Models.Catalog.FrontPage;
using Sadie.Db.Models.Catalog.Items;
using Sadie.Db.Models.Catalog.Pages;
using Sadie.Db.Models.Constants;
using Sadie.Db.Models.Furniture;
using Sadie.Db.Models.Navigator;
using Sadie.Db.Models.Players;
using Sadie.Db.Models.Players.Furniture;
using Sadie.Db.Models.Rooms;
using Sadie.Db.Models.Rooms.Chat;
using Sadie.Db.Models.Rooms.Rights;
using Sadie.Db.Models.Server;
using PlayerRelationshipType = Sadie.Db.Models.Players.PlayerRelationshipType;

namespace Sadie.Db;

public class SadieDbContext(DbContextOptions<SadieDbContext> options) : DbContext(options)
{
    public DbSet<NavigatorCategory> NavigatorCategories { get; init; }
    public DbSet<NavigatorTab> NavigatorTabs { get; init; }
    public DbSet<FurnitureItem> FurnitureItems { get; init; }
    public DbSet<CatalogItem> CatalogItems { get; init; }
    public DbSet<CatalogPage> CatalogPages { get; init; }
    public DbSet<CatalogFrontPageItem> CatalogFrontPageItems { get; init; }
    public DbSet<RoomCategory> RoomCategories { get; init; }
    public DbSet<RoomChatMessage> RoomChatMessages { get; init; }
    public DbSet<PlayerFurnitureItemPlacementData> RoomFurnitureItems { get; init; }
    public DbSet<RoomPlayerRight> RoomPlayerRights { get; init; }
    public DbSet<RoomPaintSettings> RoomPaintSettings { get; init; }
    public DbSet<RoomSettings> RoomSettings { get; init; }
    public DbSet<RoomChatSettings> RoomChatSettings { get; init; }
    public DbSet<RoomLayout> RoomLayouts { get; init; }
    public DbSet<Room> Rooms { get; init; }
    public DbSet<Player> Players { get; init; }
    public DbSet<PlayerData> PlayerData { get; init; }
    public DbSet<PlayerAvatarData> PlayerAvatarData { get; init; }
    public DbSet<PlayerFurnitureItem> PlayerFurnitureItems { get; init; }
    public DbSet<PlayerFurnitureItemLink> PlayerFurnitureItemLinks { get; init; }
    public DbSet<PlayerBadge> PlayerBadges { get; init; }
    public DbSet<Badge> Badges { get; init; }
    public DbSet<CatalogClubOffer> CatalogClubOffers { get; init; }
    public DbSet<ServerPlayerConstants> ServerPlayerConstants { get; init; }
    public DbSet<ServerRoomConstants> ServerRoomConstants { get; init; }
    public DbSet<ServerSettings> ServerSettings { get; init; }
    public DbSet<ServerPeriodicCurrencyReward> ServerPeriodicCurrencyRewards { get; init; }
    public DbSet<ServerPeriodicCurrencyRewardLog> ServerPeriodicCurrencyRewardLogs { get; init; }
    public DbSet<PlayerSsoToken> PlayerSsoToken { get; init; }
    public DbSet<PlayerNavigatorSettings> PlayerNavigatorSettings { get; init; }
    public DbSet<Subscription> Subscriptions { get; init; }
    public DbSet<PlayerSubscription> PlayerSubscriptions { get; init; }
    public DbSet<PlayerBot> PlayerBots { get; init; }
    public DbSet<RoomDimmerSettings> RoomDimmerSettings { get; init; }
    public DbSet<RoomDimmerPreset> RoomDimmerPresets { get; init; }
    public DbSet<PlayerRoomVisit> PlayerRoomVisits { get; init; }
    public DbSet<PlayerRoomLike> PlayerRoomLikes { get; init; }
    public DbSet<PlayerMessage> PlayerMessages { get; init; }
    public DbSet<PlayerBan> PlayerBans { get; init; }
    public DbSet<BannedIpAddress> BannedIpAddresses { get; init; }
    public DbSet<OauthClient> OauthClients { get; init; }
    public DbSet<PlayerWebsiteData> PlayerWebsiteData { get; init; }
    public DbSet<ServerLocaleText> ServerLocaleTexts { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NavigatorCategory>()
            .HasOne<NavigatorTab>(e => e.Tab)
            .WithMany(g => g.Categories)
            .HasForeignKey(s => s.TabId);

        modelBuilder.Entity<FurnitureItem>()
            .Property(e => e.Type)
            .HasConversion(
                v => EnumHelpers.GetEnumDescription(v),
                v => EnumHelpers.GetEnumValueFromDescription<FurnitureItemType>(v));
        
        modelBuilder.Entity<CatalogItem>()
            .HasMany(c => c.FurnitureItems)
            .WithMany(x => x.CatalogItems);

        modelBuilder.Entity<Room>()
            .HasOne<RoomPaintSettings>(e => e.PaintSettings)
            .WithOne(x => x.Room)
            .HasForeignKey<RoomPaintSettings>(x => x.RoomId);

        modelBuilder.Entity<Player>().ToTable("players");
        modelBuilder.Entity<Group>().ToTable("groups");
        modelBuilder.Entity<PlayerSsoToken>().ToTable("player_sso_tokens");
        modelBuilder.Entity<PlayerRoomLike>().ToTable("player_room_likes");
        modelBuilder.Entity<PlayerRoomBan>().ToTable("player_room_bans");
        modelBuilder.Entity<RoomTag>().ToTable("room_tags");
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
        modelBuilder.Entity<HandItem>().ToTable("hand_items");
        modelBuilder.Entity<PlayerBot>().ToTable("player_bots");
        modelBuilder.Entity<RoomDimmerPreset>().ToTable("room_dimmer_presets");
        modelBuilder.Entity<PlayerFurnitureItemPlacementData>().ToTable("player_furniture_item_placement_data");
        modelBuilder.Entity<PlayerIgnore>().ToTable("player_ignores");
        modelBuilder.Entity<PlayerRelationshipType>().ToTable("player_relationship_types");
        
        modelBuilder.Entity<PlayerAvatarData>()
            .Property(e => e.Gender)
            .HasConversion(
                v => EnumHelpers.GetEnumDescription(v),
                v => EnumHelpers.GetEnumValueFromDescription<PlayerAvatarGender>(v.ToUpper()));
        
        modelBuilder.Entity<PlayerBot>()
            .Property(e => e.Gender)
            .HasConversion(
                v => EnumHelpers.GetEnumDescription(v),
                v => EnumHelpers.GetEnumValueFromDescription<PlayerAvatarGender>(v.ToUpper()));

        modelBuilder.Entity<Subscription>().ToTable("subscriptions");

        modelBuilder.Entity<Player>()
            .HasMany(r => r.Roles)
            .WithMany(p => p.Players)
            .UsingEntity("player_role",
                l => l.HasOne(typeof(Role)).WithMany().HasForeignKey("role_id").HasPrincipalKey(nameof(Role.Id)),
                r => r.HasOne(typeof(Player)).WithMany().HasForeignKey("player_id").HasPrincipalKey(nameof(Player.Id)),
                j => j.HasKey("role_id", "player_id"));
        

        modelBuilder.Entity<Player>()
            .HasMany(r => r.Groups)
            .WithMany(p => p.Players)
            .UsingEntity("group_player",
                l => l.HasOne(typeof(Group)).WithMany().HasForeignKey("group_id").HasPrincipalKey(nameof(Group.Id)),
                r => r.HasOne(typeof(Player)).WithMany().HasForeignKey("player_id").HasPrincipalKey(nameof(Player.Id)),
                j => j.HasKey("group_id", "player_id"));

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

        modelBuilder.Entity<FurnitureItem>()
            .Navigation(x => x.HandItems)
            .AutoInclude();
        
        modelBuilder.Entity<FurnitureItem>()
            .HasMany(c => c.HandItems)
            .WithMany(x => x.FurnitureItems);

        modelBuilder.Entity<Room>()
            .HasMany(c => c.FurnitureItems)
            .WithOne(x => x.Room)
            .HasForeignKey(e => e.RoomId);

        modelBuilder.Entity<Room>()
            .Navigation(x => x.Settings)
            .AutoInclude();

        modelBuilder.Entity<PlayerFurnitureItem>()
            .Navigation(x => x.FurnitureItem)
            .AutoInclude();
        
        modelBuilder.Entity<PlayerFurnitureItemWiredData>()
            .HasMany(r => r.SelectedItems)
            .WithMany(p => p.SelectedBy)
            .UsingEntity("player_furniture_item_wired_data_items",
                l => l.HasOne(typeof(PlayerFurnitureItemPlacementData)).WithMany().HasForeignKey("player_furniture_item_placement_data_id").HasPrincipalKey(nameof(PlayerFurnitureItemPlacementData.Id)),
                r => r.HasOne(typeof(PlayerFurnitureItemWiredData)).WithMany().HasForeignKey("player_furniture_item_wired_data_id").HasPrincipalKey(nameof(PlayerFurnitureItemWiredData.Id)),
                j => j.HasKey("player_furniture_item_placement_data_id", "player_furniture_item_wired_data_id"));

        modelBuilder.Entity<PlayerFurnitureItem>()
            .Navigation(x => x.Player)
            .AutoInclude();

        modelBuilder.Entity<PlayerFurnitureItem>()
            .Navigation(x => x.PlacementData)
            .AutoInclude();

        modelBuilder.Entity<PlayerFurnitureItemPlacementData>()
            .Navigation(x => x.PlayerFurnitureItem)
            .AutoInclude();

        modelBuilder.Entity<PlayerFurnitureItemWiredData>()
            .HasOne(x => x.PlacementData)
            .WithOne(x => x.WiredData)
            .HasForeignKey<PlayerFurnitureItemWiredData>(e => e.PlayerFurnitureItemPlacementDataId);

        modelBuilder.Entity<PlayerFurnitureItemPlacementData>()
            .Navigation(x => x.WiredData)
            .AutoInclude();

        modelBuilder.Entity<Player>()
            .HasMany<PlayerBan>(x => x.Bans)
            .WithOne(x => x.Player);

        modelBuilder.Entity<PlayerFriendship>()
            .Navigation(x => x.OriginPlayer)
            .AutoInclude();

        modelBuilder.Entity<PlayerFriendship>()
            .Navigation(x => x.TargetPlayer)
            .AutoInclude();
        
        modelBuilder.Entity<Player>()
            .Navigation(x => x.AvatarData)
            .AutoInclude();

        modelBuilder.Entity<Role>()
            .Navigation(x => x.Permissions)
            .AutoInclude();
        
        modelBuilder.Entity<PlayerData>(entity =>
        {
            entity.Property(p => p.RespectPoints)
                .HasDefaultValue(15);

            entity.Property(p => p.RespectPointsPet)
                .HasDefaultValue(15);

            entity.Property(p => p.AchievementScore)
                .HasDefaultValue(15);

            entity.Property(p => p.AllowFriendRequests)
                .HasDefaultValue(true);

            entity.Property(p => p.IsOnline)
                .HasDefaultValue(false);
        });
        
        modelBuilder.Entity<RoomSettings>(entity =>
        {
            entity.Property(p => p.WalkDiagonal)
                .HasDefaultValue(true);

            entity.Property(p => p.AccessType)
                .HasDefaultValue(RoomAccessType.Open);

            entity.Property(p => p.WhoCanMute)
                .HasDefaultValue(0);

            entity.Property(p => p.WhoCanKick)
                .HasDefaultValue(0);

            entity.Property(p => p.WhoCanBan)
                .HasDefaultValue(0);

            entity.Property(p => p.AllowPets)
                .HasDefaultValue(true);

            entity.Property(p => p.CanPetsEat)
                .HasDefaultValue(true);

            entity.Property(p => p.HideWalls)
                .HasDefaultValue(false);

            entity.Property(p => p.WallThickness)
                .HasDefaultValue(0);

            entity.Property(p => p.FloorThickness)
                .HasDefaultValue(0);

            entity.Property(p => p.CanUsersOverlap)
                .HasDefaultValue(false);
        });
        
        modelBuilder.Entity<PlayerNavigatorSettings>(entity =>
        {
            entity.Property(e => e.WindowX)
                .HasDefaultValue(50);

            entity.Property(e => e.WindowY)
                .HasDefaultValue(50);

            entity.Property(e => e.WindowWidth)
                .HasDefaultValue(435);

            entity.Property(e => e.WindowHeight)
                .HasDefaultValue(535);

            entity.Property(e => e.OpenSearches)
                .HasDefaultValue(false);
        });
        
        modelBuilder.Entity<PlayerGameSettings>(entity =>
        {
            entity.Property(e => e.SystemVolume)
                .HasDefaultValue(100);

            entity.Property(e => e.FurnitureVolume)
                .HasDefaultValue(100);

            entity.Property(e => e.TraxVolume)
                .HasDefaultValue(100);

            entity.Property(e => e.PreferOldChat)
                .HasDefaultValue(false);

            entity.Property(e => e.BlockRoomInvites)
                .HasDefaultValue(false);

            entity.Property(e => e.BlockCameraFollow)
                .HasDefaultValue(false);

            entity.Property(e => e.UiFlags)
                .HasDefaultValue(1);

            entity.Property(e => e.ShowNotifications)
                .HasDefaultValue(true);
        });
        
        modelBuilder.Entity<RoomChatSettings>(entity =>
        {
            entity.Property(p => p.ChatType)
                .HasDefaultValue(0);

            entity.Property(p => p.ChatWeight)
                .HasDefaultValue(1);

            entity.Property(p => p.ChatSpeed)
                .HasDefaultValue(1);

            entity.Property(p => p.ChatDistance)
                .HasDefaultValue(50);

            entity.Property(p => p.ChatProtection)
                .HasDefaultValue(1);
        });

        modelBuilder.Entity<RoomPaintSettings>(entity =>
        {
            entity.Property(p => p.FloorPaint)
                .HasDefaultValue("0.0");

            entity.Property(p => p.WallPaint)
                .HasDefaultValue("0.0");

            entity.Property(p => p.LandscapePaint)
                .HasDefaultValue("0.0");
        });
        
        modelBuilder.Entity<PlayerFurnitureItem>(entity =>
        {
            entity.Property(p => p.LimitedData)
                .HasDefaultValue(string.Empty);

            entity.Property(p => p.MetaData)
                .HasDefaultValue(string.Empty);
        });

        modelBuilder.Entity<PlayerAvatarData>(entity =>
        {
            entity.Property(p => p.Gender)
                .HasDefaultValue(PlayerAvatarGender.Male);

            entity.Property(p => p.ChatBubbleId)
                .HasDefaultValue(ChatBubble.Default);
        });
    }
}