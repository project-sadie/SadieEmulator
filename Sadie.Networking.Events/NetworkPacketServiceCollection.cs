using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Sadie.Networking.Events.Handlers;
using Sadie.Networking.Events.Handlers.Camera;
using Sadie.Networking.Events.Handlers.Catalog;
using Sadie.Networking.Events.Handlers.Club;
using Sadie.Networking.Events.Handlers.GameCenter;
using Sadie.Networking.Events.Handlers.Generic;
using Sadie.Networking.Events.Handlers.Handshake;
using Sadie.Networking.Events.Handlers.HotelView;
using Sadie.Networking.Events.Handlers.Navigator;
using Sadie.Networking.Events.Handlers.Players;
using Sadie.Networking.Events.Handlers.Players.Club;
using Sadie.Networking.Events.Handlers.Players.Friendships;
using Sadie.Networking.Events.Handlers.Players.Inventory;
using Sadie.Networking.Events.Handlers.Players.Messenger;
using Sadie.Networking.Events.Handlers.Players.Wardrobe;
using Sadie.Networking.Events.Handlers.Rooms;
using Sadie.Networking.Events.Handlers.Rooms.Doorbell;
using Sadie.Networking.Events.Handlers.Rooms.Furniture;
using Sadie.Networking.Events.Handlers.Rooms.Rights;
using Sadie.Networking.Events.Handlers.Rooms.Users;
using Sadie.Networking.Events.Handlers.Rooms.Users.Chat;
using Sadie.Networking.Events.Handlers.Tracking;
using Sadie.Networking.Events.Handlers.Unknown;
using Sadie.Networking.Events.Parsers;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events;

public static class NetworkPacketServiceCollection
{
    public static void AddServices(IServiceCollection serviceCollection)
    {
        serviceCollection.Scan(scan => scan
            .FromAssemblyOf<INetworkPacketEventParser>()
            .AddClasses(classes => classes.AssignableTo<INetworkPacketEventParser>())
            .AsSelf()
            .WithSingletonLifetime());
        
        serviceCollection.Scan(scan => scan
            .FromAssemblyOf<INetworkPacketEventHandler>()
            .AddClasses(classes => classes.AssignableTo<INetworkPacketEventHandler>())
            .AsSelf()
            .WithSingletonLifetime());
        
        serviceCollection.AddSingleton(provider => new ConcurrentDictionary<int, INetworkPacketEventHandler>
        {
            [ClientPacketId.ClientVersion] = provider.GetRequiredService<ClientVersionEventHandler>(),
            [ClientPacketId.ClientVariables] = provider.GetRequiredService<ClientVariablesEventHandler>(),
            [ClientPacketId.UniqueId] = provider.GetRequiredService<UniqueIdEventHandler>(),
            [ClientPacketId.SecureLogin] = provider.GetRequiredService<SecureLoginEventHandler>(),
            [ClientPacketId.PerformanceLog] = provider.GetRequiredService<PerformanceLogEventHandler>(),
            [ClientPacketId.PlayerActivity] = provider.GetRequiredService<PlayerActivityEventHandler>(),
            [ClientPacketId.PlayerData] = provider.GetRequiredService<PlayerDataEventHandler>(),
            [ClientPacketId.PlayerBalance] = provider.GetRequiredService<PlayerBalanceEventHandler>(),
            [ClientPacketId.PlayerSubscription] = provider.GetRequiredService<PlayerSubscriptionEventHandler>(),
            [ClientPacketId.NavigatorData] = provider.GetRequiredService<NavigatorDataEventHandler>(),
            [ClientPacketId.PlayerFriendsList] = provider.GetRequiredService<PlayerFriendsEventHandler>(),
            [ClientPacketId.PlayerMessengerInit] = provider.GetRequiredService<PlayerMessengerInitEventHandler>(),
            [ClientPacketId.PlayerPing] = provider.GetRequiredService<PlayerPingEventHandler>(),
            [ClientPacketId.PlayerPong] = provider.GetRequiredService<PlayerPongEventHandler>(),
            [ClientPacketId.HotelViewData] = provider.GetRequiredService<HotelViewDataEventHandler>(),
            [ClientPacketId.PlayerUsername] = provider.GetRequiredService<PlayerUsernameEventHandler>(),
            [ClientPacketId.PlayerMeMenuSettings] = provider.GetRequiredService<PlayerMeMenuSettingsEventHandler>(),
            [ClientPacketId.HotelViewBonusRare] = provider.GetRequiredService<HotelViewBonusRareEventHandler>(),
            [ClientPacketId.GetBadgePointLimits] = provider.GetRequiredService<GetBadgePointLimitsEventHandler>(),
            [ClientPacketId.GameCenterRequestGames] = provider.GetRequiredService<RequestGameCenterConfigEventHandler>(),
            [ClientPacketId.GetGameAchievementsMessage] = provider.GetRequiredService<GetGameAchievementsMessageEventHandler>(),
            [ClientPacketId.PromotedRooms] = provider.GetRequiredService<PromotedRoomsEventHandler>(),
            [ClientPacketId.RoomCategories] = provider.GetRequiredService<RoomCategoriesEventHandler>(),
            [ClientPacketId.NavigatorEventCategories] = provider.GetRequiredService<NavigatorEventHandlerCategoriesEventHandler>(),
            [ClientPacketId.PlayerFriendRequestsList] = provider.GetRequiredService<PlayerFriendRequestsEventHandler>(),
            [ClientPacketId.PlayerSanctionStatus] = provider.GetRequiredService<PlayerSanctionStatusEventHandler>(),
            [ClientPacketId.GetTargetedOffer] = provider.GetRequiredService<GetTargetedOfferEventHandler>(),
            [ClientPacketId.RoomLoaded] = provider.GetRequiredService<RoomLoadedEventHandler>(),
            [ClientPacketId.GetInterstitial] = provider.GetRequiredService<GetInterstitialEventHandler>(),
            [ClientPacketId.RoomHeightmap] = provider.GetRequiredService<RoomHeightmapEventHandler>(),
            [ClientPacketId.RoomHeightmap2] = provider.GetRequiredService<RoomHeightmapEventHandler>(),
            [ClientPacketId.RoomUserChat] = provider.GetRequiredService<RoomUserChatEventHandler>(),
            [ClientPacketId.RoomUserShout] = provider.GetRequiredService<RoomUserShoutEventHandler>(),
            [ClientPacketId.RoomUserWalk] = provider.GetRequiredService<RoomUserWalkEventHandler>(),
            [ClientPacketId.RoomUserDance] = provider.GetRequiredService<RoomUserDanceEventHandler>(),
            [ClientPacketId.RoomUserAction] = provider.GetRequiredService<RoomUserActionEventHandler>(),
            [ClientPacketId.RoomUserStartTyping] = provider.GetRequiredService<RoomUserStartTypingEventHandler>(),
            [ClientPacketId.RoomUserStopTyping] = provider.GetRequiredService<RoomUserStopTypingEventHandler>(),
            [ClientPacketId.RoomUserWhisper] = provider.GetRequiredService<RoomUserWhisperEventHandler>(),
            [ClientPacketId.RoomUserLookAt] = provider.GetRequiredService<RoomUserLookAtEventHandler>(),
            [ClientPacketId.RoomUserSign] = provider.GetRequiredService<RoomUserSignEventHandler>(),
            [ClientPacketId.RoomUserSit] = provider.GetRequiredService<RoomUserSitEventHandler>(),
            [ClientPacketId.SaveNavigatorSettings] = provider.GetRequiredService<SaveNavigatorSettingsEventHandler>(),
            [ClientPacketId.NavigatorSearch] = provider.GetRequiredService<NavigatorSearchEventHandler>(),
            [ClientPacketId.PlayerChangedAppearance] = provider.GetRequiredService<PlayerChangedAppearanceEventHandler>(),
            [ClientPacketId.PlayerChangedMotto] = provider.GetRequiredService<PlayerChangedMottoEventHandler>(),
            [ClientPacketId.PlayerRelationships] = provider.GetRequiredService<PlayerRelationshipsEventHandler>(),
            [ClientPacketId.RoomUserTags] = provider.GetRequiredService<RoomUserTagsEventHandler>(),
            [ClientPacketId.RoomForwardData] = provider.GetRequiredService<RoomForwardDataEventHandler>(),
            [ClientPacketId.PlayerProfile] = provider.GetRequiredService<PlayerProfileEventHandler>(),
            [ClientPacketId.PlayerBadges] = provider.GetRequiredService<PlayerWearingBadgesEventHandler>(),
            [ClientPacketId.RoomUserRespect] = provider.GetRequiredService<RoomUserRespectEventHandler>(),
            [ClientPacketId.PlayerFriendRequest] = provider.GetRequiredService<PlayerSendFriendRequestEventHandler>(), 
            [ClientPacketId.PlayerAcceptFriendRequest] = provider.GetRequiredService<PlayerAcceptFriendRequestEventHandler>(),
            [ClientPacketId.PlayerDeclineFriendRequest] = provider.GetRequiredService<PlayerDeclineFriendRequestEventHandler>(),
            [ClientPacketId.PlayerRemoveFriend] = provider.GetRequiredService<PlayerRemoveFriendsEventHandler>(),
            [ClientPacketId.PlayerChangeRelation] = provider.GetRequiredService<PlayerChangeRelationshipEventHandler>(),
            [ClientPacketId.PlayerCreateRoom] = provider.GetRequiredService<PlayerCreateRoomEventHandler>(),
            [ClientPacketId.HabboClubData] = provider.GetRequiredService<PlayerClubOffersEventHandler>(),
            [ClientPacketId.HabboClubCenter] = provider.GetRequiredService<PlayerClubCenterDataEventHandler>(),
            [ClientPacketId.HabboClubGifts] = provider.GetRequiredService<HabboClubGiftsEventHandler>(),
            [ClientPacketId.RoomUserGoToHotelView] = provider.GetRequiredService<RoomUserGoToHotelViewEventHandler>(),
            [ClientPacketId.PlayerSearch] = provider.GetRequiredService<PlayerSearchEventHandler>(),
            [ClientPacketId.PlayerStalk] = provider.GetRequiredService<PlayerStalkEventHandler>(),
            [ClientPacketId.PlayerSendMessage] = provider.GetRequiredService<PlayerSendDirectMessageEventHandler>(),
            [ClientPacketId.RoomSettings] = provider.GetRequiredService<RequestRoomSettingsEventHandler>(),
            [ClientPacketId.RoomSettingsSave] = provider.GetRequiredService<RoomSettingsSaveEventHandler>(),
            [ClientPacketId.PlayerInventoryBadges] = provider.GetRequiredService<PlayerInventoryBadgesEventHandler>(),
            [ClientPacketId.RoomDoorbellAnswer] = provider.GetRequiredService<RoomDoorbellAnswerEventHandler>(),
            [ClientPacketId.RoomDoorbellAccepted] = provider.GetRequiredService<RoomDoorbellAcceptedEventHandler>(),
            [ClientPacketId.PlayerAchievements] = provider.GetRequiredService<PlayerAchievementsEventHandler>(),
            [ClientPacketId.CameraPrice] = provider.GetRequiredService<CameraPriceEventHandler>(),
            [ClientPacketId.CatalogMode] = provider.GetRequiredService<CatalogModeEventHandler>(),
            [ClientPacketId.CatalogMarketplaceConfig] = provider.GetRequiredService<CatalogMarketplaceConfigEventHandler>(),
            [ClientPacketId.CatalogRecyclerLogic] = provider.GetRequiredService<CatalogRecyclerLogicEventHandler>(),
            [ClientPacketId.CatalogGiftConfig] = provider.GetRequiredService<CatalogGiftConfigEventHandler>(),
            [ClientPacketId.ChangeChatBubble] = provider.GetRequiredService<RoomUserChangeChatBubbleEventHandler>(),
            [ClientPacketId.CatalogDiscount] = provider.GetRequiredService<CatalogDiscountEventHandler>(),
            [ClientPacketId.CatalogIndex] = provider.GetRequiredService<CatalogIndexEventHandler>(),
            [ClientPacketId.CatalogPage] = provider.GetRequiredService<CatalogPageEventHandler>(),
            [ClientPacketId.CatalogPurchase] = provider.GetRequiredService<CatalogPurchaseEventHandler>(),
            [ClientPacketId.PlayerFriendListUpdate] = provider.GetRequiredService<PlayerFriendListUpdateEventHandler>(),
            [ClientPacketId.HotelViewPromotions] = provider.GetRequiredService<HotelViewPromotionsEventHandler>(),
            [ClientPacketId.RoomLike] = provider.GetRequiredService<RoomLikeEventHandler>(),
            [ClientPacketId.PlayerInventoryFurnitureItems] = provider.GetRequiredService<PlayerInventoryFurnitureItemsEventHandler>(),
            [ClientPacketId.RoomFurnitureItemPlaced] = provider.GetRequiredService<RoomItemPlacedEventHandler>(),
            [ClientPacketId.RoomFurnitureItemEjected] = provider.GetRequiredService<RoomItemEjectedEventHandler>(),
            [ClientPacketId.RoomFloorFurnitureItemUpdated] = provider.GetRequiredService<RoomFloorItemUpdatedEventHandler>(),
            [ClientPacketId.RoomFurnitureItemToggle] = provider.GetRequiredService<RoomItemUseEventHandler>(),
            [ClientPacketId.RoomGiveUserRights] = provider.GetRequiredService<RoomGiveUserRightsEventHandler>(),
            [ClientPacketId.RoomRemoveUserRights] = provider.GetRequiredService<RoomRemoveUserRightsEventHandler>(),
            [ClientPacketId.PlayerWardrobe] = provider.GetRequiredService<PlayerWardrobeEventHandler>(),
            [ClientPacketId.PlayerWardrobeSave] = provider.GetRequiredService<PlayerWardrobeSaveEventHandler>(),
            [ClientPacketId.RoomWallFurnitureItemUpdated] = provider.GetRequiredService<RoomWallItemUpdatedEventHandler>()
        });
        
        serviceCollection.AddSingleton<INetworkPacketHandler, ClientPacketHandler>();
    }
}