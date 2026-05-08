using Microsoft.EntityFrameworkCore;
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
    public DbSet<PlayerWardrobeItem> PlayerWardrobeItems { get; set; }
    public DbSet<PlayerRespect> PlayerRespects { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SadieDbContext).Assembly);
    }
}