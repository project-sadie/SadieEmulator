using Microsoft.EntityFrameworkCore;
using Sadie.Database.Models;
using Sadie.Database.Models.Catalog;
using Sadie.Database.Models.Catalog.FrontPage;
using Sadie.Database.Models.Catalog.Items;
using Sadie.Database.Models.Catalog.Pages;
using Sadie.Database.Models.Furniture;
using Sadie.Database.Models.Navigator;
using Sadie.Database.Models.Players;
using Sadie.Database.Models.Rooms;
using Sadie.Database.Models.Rooms.Chat;
using Sadie.Database.Models.Rooms.Furniture;
using Sadie.Database.Models.Rooms.Rights;
using Sadie.Shared.Helpers;
using Sadie.Shared.Unsorted;
using Sadie.Shared.Unsorted.Game.Avatar;

namespace Sadie.Database;

public class SadieContext(DbContextOptions<SadieContext> options) : DbContext(options)
{
    public DbSet<NavigatorCategory> NavigatorCategories { get; set; }
    public DbSet<NavigatorTab> NavigatorTabs { get; set; }
    public DbSet<FurnitureItem> FurnitureItems { get; set; }
    public DbSet<CatalogItem> CatalogItems { get; set; }
    public DbSet<CatalogPage> CatalogPages { get; set; }
    public DbSet<CatalogFrontPageItem> CatalogFrontPageItems { get; set; }
    public DbSet<RoomCategory> RoomCategories { get; set; }
    public DbSet<RoomChatMessage> RoomChatMessages { get; set; }
    public DbSet<RoomFurnitureItem> RoomFurnitureItems { get; set; }
    public DbSet<RoomPlayerRight> RoomPlayerRights { get; set; }
    public DbSet<RoomSettings> RoomSettings { get; set; }
    public DbSet<RoomLayout> RoomLayouts { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Player> Players { get; set; }
    public DbSet<PlayerData> PlayerData { get; set; }
    public DbSet<PlayerFurnitureItem> PlayerFurnitureItems { get; set; }
    public DbSet<PlayerBadge> PlayerBadges { get; set; }
    public DbSet<Badge> Badges { get; set; }
    public DbSet<CatalogClubOffer> CatalogClubOffers { get; set; }
    
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
    }
}