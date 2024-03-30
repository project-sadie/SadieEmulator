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
using Sadie.Networking.Events.Parsers.Catalog;
using Sadie.Networking.Events.Parsers.Club;
using Sadie.Networking.Events.Parsers.Generic;
using Sadie.Networking.Events.Parsers.Handshake;
using Sadie.Networking.Events.Parsers.Navigator;
using Sadie.Networking.Events.Parsers.Players;
using Sadie.Networking.Events.Parsers.Players.Club;
using Sadie.Networking.Events.Parsers.Players.Friendships;
using Sadie.Networking.Events.Parsers.Players.Messenger;
using Sadie.Networking.Events.Parsers.Players.Wardrobe;
using Sadie.Networking.Events.Parsers.Rooms;
using Sadie.Networking.Events.Parsers.Rooms.Doorbell;
using Sadie.Networking.Events.Parsers.Rooms.Furniture;
using Sadie.Networking.Events.Parsers.Rooms.Rights;
using Sadie.Networking.Events.Parsers.Rooms.Users;
using Sadie.Networking.Events.Parsers.Rooms.Users.Chat;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events;

public static class NetworkPacketServiceCollection
{
    public static void AddServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<RoomSettingsSaveParser>();
        serviceCollection.AddSingleton<RoomUserWalkParser>();
        serviceCollection.AddSingleton<RoomUserRespectParser>();
        serviceCollection.AddSingleton<RoomUserActionParser>();
        serviceCollection.AddSingleton<RoomUserDanceParser>();
        serviceCollection.AddSingleton<RoomUserLookAtParser>();
        serviceCollection.AddSingleton<RoomUserSignParser>();
        serviceCollection.AddSingleton<RoomUserTagsParser>();
        serviceCollection.AddSingleton<RoomForwardDataParser>();
        serviceCollection.AddSingleton<RoomLoadedParser>();
        serviceCollection.AddSingleton<RoomUserWhisperParser>();
        serviceCollection.AddSingleton<CatalogPurchaseParser>();
        serviceCollection.AddSingleton<PlayerRemoveFriendsParser>();
        serviceCollection.AddSingleton<RoomUserChatParser>();
        serviceCollection.AddSingleton<CatalogModeParser>();
        serviceCollection.AddSingleton<RoomUserChangeChatBubbleParser>();
        serviceCollection.AddSingleton<CatalogPageParser>();
        serviceCollection.AddSingleton<PlayerChangeRelationshipParser>();
        serviceCollection.AddSingleton<RequestRoomSettingsParser>();
        serviceCollection.AddSingleton<PlayerCreateRoomParser>();
        serviceCollection.AddSingleton<PlayerWearingBadgesParser>();
        serviceCollection.AddSingleton<RoomDoorbellAcceptedParser>();
        serviceCollection.AddSingleton<PlayerRelationshipsParser>();
        serviceCollection.AddSingleton<PlayerWardrobeSaveParser>();
        serviceCollection.AddSingleton<NavigatorSearchParser>();
        serviceCollection.AddSingleton<RoomRemoveUserRightsParser>();
        serviceCollection.AddSingleton<RoomFurnitureItemUseParser>();
        serviceCollection.AddSingleton<MachineIdParser>();
        serviceCollection.AddSingleton<PlayerSendFriendRequestParser>();
        serviceCollection.AddSingleton<PlayerClubMembershipParser>();
        serviceCollection.AddSingleton<RoomGiveUserRightsParser>();
        serviceCollection.AddSingleton<HabboClubDataParser>();
        serviceCollection.AddSingleton<PlayerSearchParser>();
        serviceCollection.AddSingleton<RoomWallFurnitureItemUpdatedParser>();
        serviceCollection.AddSingleton<SaveNavigatorSettingsParser>();
        serviceCollection.AddSingleton<PlayerStalkParser>();
        serviceCollection.AddSingleton<PlayerDeclineFriendRequestParser>();
        serviceCollection.AddSingleton<PlayerPingParser>();
        serviceCollection.AddSingleton<SecureLoginParser>();
        serviceCollection.AddSingleton<PlayerAcceptFriendRequestParser>();
        serviceCollection.AddSingleton<RoomFurnitureItemEjectedParser>();
        serviceCollection.AddSingleton<PlayerChangedAppearanceParser>();
        serviceCollection.AddSingleton<RoomFloorFurnitureItemUpdatedParser>();
        serviceCollection.AddSingleton<RoomFurnitureItemPlacedParser>();
        serviceCollection.AddSingleton<PlayerChangedMottoParser>();
        
        serviceCollection.AddSingleton<ClientVersionEvent>();
        serviceCollection.AddSingleton<ClientVariablesEvent>();
        serviceCollection.AddSingleton<MachineIdEvent>();
        serviceCollection.AddSingleton<SecureLoginEvent>();
        serviceCollection.AddSingleton<PerformanceLogEvent>();
        serviceCollection.AddSingleton<PlayerActivityEvent>();
        serviceCollection.AddSingleton<PlayerDataEvent>();
        serviceCollection.AddSingleton<PlayerBalanceEvent>();
        serviceCollection.AddSingleton<PlayerClubMembershipEvent>();
        serviceCollection.AddSingleton<NavigatorDataEvent>();
        serviceCollection.AddSingleton<PlayerFriendsEvent>();
        serviceCollection.AddSingleton<PlayerPingEvent>();
        serviceCollection.AddSingleton<PlayerPongEvent>();
        serviceCollection.AddSingleton<HotelViewDataEvent>();
        serviceCollection.AddSingleton<PlayerUsernameEvent>();
        serviceCollection.AddSingleton<PlayerMeMenuSettingsEvent>();
        serviceCollection.AddSingleton<HotelViewBonusRareEvent>();
        serviceCollection.AddSingleton<UnknownEvent1>();
        serviceCollection.AddSingleton<RequestGameCenterConfigEvent>();
        serviceCollection.AddSingleton<UnknownEvent4>();
        serviceCollection.AddSingleton<PromotedRoomsEvent>();
        serviceCollection.AddSingleton<RoomCategoriesEvent>();
        serviceCollection.AddSingleton<NavigatorEventCategoriesEvent>();
        serviceCollection.AddSingleton<PlayerFriendRequestsEvent>();
        serviceCollection.AddSingleton<PlayerSanctionStatusEvent>();
        serviceCollection.AddSingleton<UnknownEvent2>();
        serviceCollection.AddSingleton<RoomLoadedEvent>();
        serviceCollection.AddSingleton<UnknownEvent3>();
        serviceCollection.AddSingleton<RoomHeightmapEvent>();
        serviceCollection.AddSingleton<RoomUserChatEvent>();
        serviceCollection.AddSingleton<RoomUserShoutEvent>();
        serviceCollection.AddSingleton<RoomUserWalkEvent>();
        serviceCollection.AddSingleton<RoomUserDanceEvent>();
        serviceCollection.AddSingleton<RoomUserActionEvent>();
        serviceCollection.AddSingleton<RoomUserActionEvent>();
        serviceCollection.AddSingleton<RoomUserStartTypingEvent>();
        serviceCollection.AddSingleton<RoomUserStopTypingEvent>();
        serviceCollection.AddSingleton<RoomUserWhisperEvent>();
        serviceCollection.AddSingleton<RoomUserLookAtEvent>();
        serviceCollection.AddSingleton<RoomUserSignEvent>();
        serviceCollection.AddSingleton<RoomUserSitEvent>();
        serviceCollection.AddSingleton<SaveNavigatorSettingsEvent>();
        serviceCollection.AddSingleton<NavigatorSearchEvent>();
        serviceCollection.AddSingleton<PlayerChangedAppearanceEvent>();
        serviceCollection.AddSingleton<PlayerChangedMottoEvent>();
        serviceCollection.AddSingleton<PlayerRelationshipsEvent>();
        serviceCollection.AddSingleton<RoomUserTagsEvent>();
        serviceCollection.AddSingleton<RoomForwardDataEvent>();
        serviceCollection.AddSingleton<PlayerProfileEvent>();
        serviceCollection.AddSingleton<PlayerWearingBadgesEvent>();
        serviceCollection.AddSingleton<RoomUserRespectEvent>();
        serviceCollection.AddSingleton<PlayerSendFriendRequestEvent>();
        serviceCollection.AddSingleton<PlayerAcceptFriendRequestEvent>();
        serviceCollection.AddSingleton<PlayerDeclineFriendRequestEvent>();
        serviceCollection.AddSingleton<PlayerRemoveFriendsEvent>();
        serviceCollection.AddSingleton<PlayerChangeRelationshipEvent>();
        serviceCollection.AddSingleton<PlayerCreateRoomEvent>();
        serviceCollection.AddSingleton<HabboClubDataEvent>();
        serviceCollection.AddSingleton<PlayerClubCenterDataEvent>();
        serviceCollection.AddSingleton<HabboClubGiftsEvent>();
        serviceCollection.AddSingleton<RoomUserGoToHotelViewEvent>();
        serviceCollection.AddSingleton<PlayerSearchEvent>();
        serviceCollection.AddSingleton<PlayerStalkEvent>();
        serviceCollection.AddSingleton<PlayerSendDirectMessageEvent>();
        serviceCollection.AddSingleton<RequestRoomSettingsEvent>();
        serviceCollection.AddSingleton<RoomSettingsSaveEvent>();
        serviceCollection.AddSingleton<PlayerInventoryBadgesEvent>();
        serviceCollection.AddSingleton<RoomDoorbellAnswerEvent>();
        serviceCollection.AddSingleton<RoomDoorbellAcceptedEvent>();
        serviceCollection.AddSingleton<PlayerAchievementsEvent>();
        serviceCollection.AddSingleton<CameraPriceEvent>();
        serviceCollection.AddSingleton<CatalogModeEvent>();
        serviceCollection.AddSingleton<CatalogMarketplaceConfigEvent>();
        serviceCollection.AddSingleton<CatalogRecyclerLogicEvent>();
        serviceCollection.AddSingleton<CatalogGiftConfigEvent>();
        serviceCollection.AddSingleton<RoomUserChangeChatBubbleEvent>();
        serviceCollection.AddSingleton<CatalogDiscountEvent>();
        serviceCollection.AddSingleton<CatalogIndexEvent>();
        serviceCollection.AddSingleton<CatalogPageEvent>();
        serviceCollection.AddSingleton<CatalogPurchaseEvent>();
        serviceCollection.AddSingleton<RoomUserChatEvent>();
        serviceCollection.AddSingleton<RoomUserShoutEvent>();
        serviceCollection.AddSingleton<NavigatorSearchEvent>();
        serviceCollection.AddSingleton<PlayerFriendListUpdateEvent>();
        serviceCollection.AddSingleton<HotelViewPromotionsEvent>();
        serviceCollection.AddSingleton<RoomLikeEvent>();
        serviceCollection.AddSingleton<PlayerInventoryFurnitureItemsEvent>();
        serviceCollection.AddSingleton<RoomFurnitureItemPlacedEvent>();
        serviceCollection.AddSingleton<RoomFurnitureItemEjectedEvent>();
        serviceCollection.AddSingleton<RoomFloorFurnitureItemUpdatedEvent>();
        serviceCollection.AddSingleton<RoomFurnitureItemUseEvent>();
        serviceCollection.AddSingleton<PlayerMessengerInitEvent>();
        serviceCollection.AddSingleton<RoomGiveUserRightsEvent>();
        serviceCollection.AddSingleton<RoomRemoveUserRightsEvent>();
        serviceCollection.AddSingleton<PlayerWardrobeEvent>();
        serviceCollection.AddSingleton<PlayerWardrobeSaveEvent>();
        serviceCollection.AddSingleton<RoomWallFurnitureItemUpdatedEvent>();
        
        serviceCollection.AddSingleton(provider => new ConcurrentDictionary<int, INetworkPacketEvent>
        {
            [ClientPacketId.ClientVersion] = provider.GetRequiredService<ClientVersionEvent>(),
            [ClientPacketId.ClientVariables] = provider.GetRequiredService<ClientVariablesEvent>(),
            [ClientPacketId.MachineId] = provider.GetRequiredService<MachineIdEvent>(),
            [ClientPacketId.SecureLogin] = provider.GetRequiredService<SecureLoginEvent>(),
            [ClientPacketId.PerformanceLog] = provider.GetRequiredService<PerformanceLogEvent>(),
            [ClientPacketId.PlayerActivity] = provider.GetRequiredService<PlayerActivityEvent>(),
            [ClientPacketId.PlayerData] = provider.GetRequiredService<PlayerDataEvent>(),
            [ClientPacketId.PlayerBalance] = provider.GetRequiredService<PlayerBalanceEvent>(),
            [ClientPacketId.PlayerClubMembership] = provider.GetRequiredService<PlayerClubMembershipEvent>(),
            [ClientPacketId.NavigatorData] = provider.GetRequiredService<NavigatorDataEvent>(),
            [ClientPacketId.PlayerFriendsList] = provider.GetRequiredService<PlayerFriendsEvent>(),
            [ClientPacketId.PlayerMessengerInit] = provider.GetRequiredService<PlayerMessengerInitEvent>(),
            [ClientPacketId.PlayerPing] = provider.GetRequiredService<PlayerPingEvent>(),
            [ClientPacketId.PlayerPong] = provider.GetRequiredService<PlayerPongEvent>(),
            [ClientPacketId.HotelViewData] = provider.GetRequiredService<HotelViewDataEvent>(),
            [ClientPacketId.PlayerUsername] = provider.GetRequiredService<PlayerUsernameEvent>(),
            [ClientPacketId.PlayerMeMenuSettings] = provider.GetRequiredService<PlayerMeMenuSettingsEvent>(),
            [ClientPacketId.HotelViewBonusRare] = provider.GetRequiredService<HotelViewBonusRareEvent>(),
            [ClientPacketId.UnknownEvent1] = provider.GetRequiredService<UnknownEvent1>(),
            [ClientPacketId.GameCenterRequestGames] = provider.GetRequiredService<RequestGameCenterConfigEvent>(),
            [ClientPacketId.UnknownEvent4] = provider.GetRequiredService<UnknownEvent4>(),
            [ClientPacketId.PromotedRooms] = provider.GetRequiredService<PromotedRoomsEvent>(),
            [ClientPacketId.RoomCategories] = provider.GetRequiredService<RoomCategoriesEvent>(),
            [ClientPacketId.NavigatorEventCategories] = provider.GetRequiredService<NavigatorEventCategoriesEvent>(),
            [ClientPacketId.PlayerFriendRequestsList] = provider.GetRequiredService<PlayerFriendRequestsEvent>(),
            [ClientPacketId.PlayerSanctionStatus] = provider.GetRequiredService<PlayerSanctionStatusEvent>(),
            [ClientPacketId.UnknownEvent2] = provider.GetRequiredService<UnknownEvent2>(),
            [ClientPacketId.RoomLoaded] = provider.GetRequiredService<RoomLoadedEvent>(),
            [ClientPacketId.UnknownEvent3] = provider.GetRequiredService<UnknownEvent3>(),
            [ClientPacketId.RoomHeightmap] = provider.GetRequiredService<RoomHeightmapEvent>(),
            [ClientPacketId.RoomHeightmap2] = provider.GetRequiredService<RoomHeightmapEvent>(),
            [ClientPacketId.RoomUserChat] = provider.GetRequiredService<RoomUserChatEvent>(),
            [ClientPacketId.RoomUserShout] = provider.GetRequiredService<RoomUserShoutEvent>(),
            [ClientPacketId.RoomUserWalk] = provider.GetRequiredService<RoomUserWalkEvent>(),
            [ClientPacketId.RoomUserDance] = provider.GetRequiredService<RoomUserDanceEvent>(),
            [ClientPacketId.RoomUserAction] = provider.GetRequiredService<RoomUserActionEvent>(),
            [ClientPacketId.RoomUserStartTyping] = provider.GetRequiredService<RoomUserStartTypingEvent>(),
            [ClientPacketId.RoomUserStopTyping] = provider.GetRequiredService<RoomUserStopTypingEvent>(),
            [ClientPacketId.RoomUserWhisper] = provider.GetRequiredService<RoomUserWhisperEvent>(),
            [ClientPacketId.RoomUserLookAt] = provider.GetRequiredService<RoomUserLookAtEvent>(),
            [ClientPacketId.RoomUserSign] = provider.GetRequiredService<RoomUserSignEvent>(),
            [ClientPacketId.RoomUserSit] = provider.GetRequiredService<RoomUserSitEvent>(),
            [ClientPacketId.SaveNavigatorSettings] = provider.GetRequiredService<SaveNavigatorSettingsEvent>(),
            [ClientPacketId.NavigatorSearch] = provider.GetRequiredService<NavigatorSearchEvent>(),
            [ClientPacketId.PlayerChangedAppearance] = provider.GetRequiredService<PlayerChangedAppearanceEvent>(),
            [ClientPacketId.PlayerChangedMotto] = provider.GetRequiredService<PlayerChangedMottoEvent>(),
            [ClientPacketId.PlayerRelationships] = provider.GetRequiredService<PlayerRelationshipsEvent>(),
            [ClientPacketId.RoomUserTags] = provider.GetRequiredService<RoomUserTagsEvent>(),
            [ClientPacketId.RoomForwardData] = provider.GetRequiredService<RoomForwardDataEvent>(),
            [ClientPacketId.PlayerProfile] = provider.GetRequiredService<PlayerProfileEvent>(),
            [ClientPacketId.PlayerBadges] = provider.GetRequiredService<PlayerWearingBadgesEvent>(),
            [ClientPacketId.RoomUserRespect] = provider.GetRequiredService<RoomUserRespectEvent>(),
            [ClientPacketId.PlayerFriendRequest] = provider.GetRequiredService<PlayerSendFriendRequestEvent>(), 
            [ClientPacketId.PlayerAcceptFriendRequest] = provider.GetRequiredService<PlayerAcceptFriendRequestEvent>(),
            [ClientPacketId.PlayerDeclineFriendRequest] = provider.GetRequiredService<PlayerDeclineFriendRequestEvent>(),
            [ClientPacketId.PlayerRemoveFriend] = provider.GetRequiredService<PlayerRemoveFriendsEvent>(),
            [ClientPacketId.PlayerChangeRelation] = provider.GetRequiredService<PlayerChangeRelationshipEvent>(),
            [ClientPacketId.PlayerCreateRoom] = provider.GetRequiredService<PlayerCreateRoomEvent>(),
            [ClientPacketId.HabboClubData] = provider.GetRequiredService<HabboClubDataEvent>(),
            [ClientPacketId.HabboClubCenter] = provider.GetRequiredService<PlayerClubCenterDataEvent>(),
            [ClientPacketId.HabboClubGifts] = provider.GetRequiredService<HabboClubGiftsEvent>(),
            [ClientPacketId.RoomUserGoToHotelView] = provider.GetRequiredService<RoomUserGoToHotelViewEvent>(),
            [ClientPacketId.PlayerSearch] = provider.GetRequiredService<PlayerSearchEvent>(),
            [ClientPacketId.PlayerStalk] = provider.GetRequiredService<PlayerStalkEvent>(),
            [ClientPacketId.PlayerSendMessage] = provider.GetRequiredService<PlayerSendDirectMessageEvent>(),
            [ClientPacketId.RoomSettings] = provider.GetRequiredService<RequestRoomSettingsEvent>(),
            [ClientPacketId.RoomSettingsSave] = provider.GetRequiredService<RoomSettingsSaveEvent>(),
            [ClientPacketId.PlayerInventoryBadges] = provider.GetRequiredService<PlayerInventoryBadgesEvent>(),
            [ClientPacketId.RoomDoorbellAnswer] = provider.GetRequiredService<RoomDoorbellAnswerEvent>(),
            [ClientPacketId.RoomDoorbellAccepted] = provider.GetRequiredService<RoomDoorbellAcceptedEvent>(),
            [ClientPacketId.PlayerAchievements] = provider.GetRequiredService<PlayerAchievementsEvent>(),
            [ClientPacketId.CameraPrice] = provider.GetRequiredService<CameraPriceEvent>(),
            [ClientPacketId.CatalogMode] = provider.GetRequiredService<CatalogModeEvent>(),
            [ClientPacketId.CatalogMarketplaceConfig] = provider.GetRequiredService<CatalogMarketplaceConfigEvent>(),
            [ClientPacketId.CatalogRecyclerLogic] = provider.GetRequiredService<CatalogRecyclerLogicEvent>(),
            [ClientPacketId.CatalogGiftConfig] = provider.GetRequiredService<CatalogGiftConfigEvent>(),
            [ClientPacketId.ChangeChatBubble] = provider.GetRequiredService<RoomUserChangeChatBubbleEvent>(),
            [ClientPacketId.CatalogDiscount] = provider.GetRequiredService<CatalogDiscountEvent>(),
            [ClientPacketId.CatalogIndex] = provider.GetRequiredService<CatalogIndexEvent>(),
            [ClientPacketId.CatalogPage] = provider.GetRequiredService<CatalogPageEvent>(),
            [ClientPacketId.CatalogPurchase] = provider.GetRequiredService<CatalogPurchaseEvent>(),
            [ClientPacketId.PlayerFriendListUpdate] = provider.GetRequiredService<PlayerFriendListUpdateEvent>(),
            [ClientPacketId.HotelViewPromotions] = provider.GetRequiredService<HotelViewPromotionsEvent>(),
            [ClientPacketId.RoomLike] = provider.GetRequiredService<RoomLikeEvent>(),
            [ClientPacketId.PlayerInventoryFurnitureItems] = provider.GetRequiredService<PlayerInventoryFurnitureItemsEvent>(),
            [ClientPacketId.RoomFurnitureItemPlaced] = provider.GetRequiredService<RoomFurnitureItemPlacedEvent>(),
            [ClientPacketId.RoomFurnitureItemEjected] = provider.GetRequiredService<RoomFurnitureItemEjectedEvent>(),
            [ClientPacketId.RoomFloorFurnitureItemUpdated] = provider.GetRequiredService<RoomFloorFurnitureItemUpdatedEvent>(),
            [ClientPacketId.RoomFurnitureItemToggle] = provider.GetRequiredService<RoomFurnitureItemUseEvent>(),
            [ClientPacketId.RoomGiveUserRights] = provider.GetRequiredService<RoomGiveUserRightsEvent>(),
            [ClientPacketId.RoomRemoveUserRights] = provider.GetRequiredService<RoomRemoveUserRightsEvent>(),
            [ClientPacketId.PlayerWardrobe] = provider.GetRequiredService<PlayerWardrobeEvent>(),
            [ClientPacketId.PlayerWardrobeSave] = provider.GetRequiredService<PlayerWardrobeSaveEvent>(),
            [ClientPacketId.RoomWallFurnitureItemUpdated] = provider.GetRequiredService<RoomWallFurnitureItemUpdatedEvent>(),
        });
        
        serviceCollection.AddSingleton<INetworkPacketHandler, ClientPacketHandler>();
    }
}