using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Sadie.Game.Navigator;
using Sadie.Game.Navigator.Tabs;
using Sadie.Game.Players;
using Sadie.Game.Players.Friendships;
using Sadie.Game.Players.Respect;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Categories;
using Sadie.Networking.Events.Camera;
using Sadie.Networking.Events.Catalog;
using Sadie.Networking.Events.Club;
using Sadie.Networking.Events.GameCenter;
using Sadie.Networking.Events.Generic;
using Sadie.Networking.Events.Handshake;
using Sadie.Networking.Events.HotelView;
using Sadie.Networking.Events.Navigator;
using Sadie.Networking.Events.Players;
using Sadie.Networking.Events.Players.Club;
using Sadie.Networking.Events.Players.Friendships;
using Sadie.Networking.Events.Players.Inventory;
using Sadie.Networking.Events.Players.Messenger;
using Sadie.Networking.Events.Rooms;
using Sadie.Networking.Events.Rooms.Access;
using Sadie.Networking.Events.Rooms.Users;
using Sadie.Networking.Events.Rooms.Users.Chat;
using Sadie.Networking.Events.Tracking;
using Sadie.Networking.Events.Unknown;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events;

public static class NetworkPacketServiceCollection
{
    public static void AddServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<SecureLoginEvent>();
        serviceCollection.AddSingleton<PlayerActivityEvent>();
        serviceCollection.AddSingleton<PlayerChangedMottoEvent>();
        serviceCollection.AddSingleton<PlayerRelationshipsEvent>();
        serviceCollection.AddSingleton<RoomUserTagsEvent>();
        serviceCollection.AddSingleton<RoomForwardDataEvent>();
        serviceCollection.AddSingleton<PlayerProfileEvent>();
        serviceCollection.AddSingleton<PlayerWearingBadgesEvent>();
        serviceCollection.AddSingleton<PlayerChangeRelationEvent>();
        serviceCollection.AddSingleton<PlayerCreateRoomEvent>();
        serviceCollection.AddSingleton<HabboClubDataEvent>();
        serviceCollection.AddSingleton<PlayerClubCenterDataEvent>();
        serviceCollection.AddSingleton<HabboClubGiftsEvent>();
        serviceCollection.AddSingleton<RoomUserGoToHotelViewEvent>();
        serviceCollection.AddSingleton<PlayerSearchEvent>();
        serviceCollection.AddSingleton<PlayerStalkEvent>();
        serviceCollection.AddSingleton<PlayerSendDirectMessageEvent>();
        serviceCollection.AddSingleton<RoomSettingsEvent>();
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
        serviceCollection.AddSingleton<PlayerFriendListUpdateEvent>();
        
        serviceCollection.AddSingleton<INetworkPacketHandler, ClientPacketHandler>();
        
        serviceCollection.AddSingleton(provider => new ConcurrentDictionary<int, INetworkPacketEvent>
        {
            [ClientPacketId.ClientVersion] = new ClientVersionEvent(),
            [ClientPacketId.ClientVariables] = new ClientVariablesEvent(),
            [ClientPacketId.MachineId] = new MachineIdEvent(),
            [ClientPacketId.SecureLogin] = provider.GetRequiredService<SecureLoginEvent>(),
            [ClientPacketId.PerformanceLog] = new PerformanceLogEvent(),
            [ClientPacketId.PlayerActivity] = provider.GetRequiredService<PlayerActivityEvent>(),
            [ClientPacketId.PlayerData] = new PlayerDataEvent(),
            [ClientPacketId.PlayerBalance] = new PlayerBalanceEvent(),
            [ClientPacketId.PlayerClubMembership] = new PlayerClubMembershipEvent(),
            [ClientPacketId.NavigatorData] = new NavigatorDataEvent(),
            
            [ClientPacketId.PlayerFriendsList] = new PlayerFriendsEvent(
                provider.GetRequiredService<IPlayerRepository>(), 
                provider.GetRequiredService<IRoomRepository>()),
            
            [ClientPacketId.PlayerMessengerInit] = new PlayerMessengerInitEvent(
                provider.GetRequiredService<IPlayerRepository>(), 
                provider.GetRequiredService<IRoomRepository>(), 
                provider.GetRequiredService<PlayerConstants>()),
            
            [ClientPacketId.PlayerPing] = new PlayerPingEvent(),
            [ClientPacketId.PlayerPong] = new PlayerPongEvent(),
            [ClientPacketId.HotelViewData] = new HotelViewDataEvent(),
            [ClientPacketId.PlayerUsername] = new PlayerUsernameEvent(),
            [ClientPacketId.PlayerMeMenuSettings] = new PlayerMeMenuSettingsEvent(),
            [ClientPacketId.HotelViewBonusRare] = new HotelViewBonusRareEvent(),
            [ClientPacketId.UnknownEvent1] = new UnknownEvent1(),
            [ClientPacketId.GameCenterRequestGames] = new RequestGameCenterConfigEvent(),
            [ClientPacketId.UnknownEvent4] = new UnknownEvent4(),
            [ClientPacketId.PromotedRooms] = new PromotedRoomsEvent(),
            [ClientPacketId.RoomCategories] = new RoomCategoriesEvent(provider.GetRequiredService<IRoomCategoryRepository>()),
            [ClientPacketId.NavigatorEventCategories] = new NavigatorEventCategoriesEvent(),
            
            [ClientPacketId.PlayerFriendRequestsList] = new PlayerFriendRequestsEvent(
                provider.GetRequiredService<IPlayerFriendshipRepository>()),
            
            [ClientPacketId.PlayerSanctionStatus] = new PlayerSanctionStatusEvent(),
            [ClientPacketId.UnknownEvent2] = new UnknownEvent2(),
            [ClientPacketId.RoomLoaded] = ActivatorUtilities.CreateInstance<RoomLoadedEvent>(provider),
            [ClientPacketId.UnknownEvent3] = new UnknownEvent3(),
            [ClientPacketId.RoomHeightmap] = new RoomHeightmapEvent(provider.GetRequiredService<IRoomRepository>()),
            [ClientPacketId.RoomHeightmap2] = new RoomHeightmapEvent(provider.GetRequiredService<IRoomRepository>()),
            
            [ClientPacketId.RoomUserChat] = provider.GetRequiredService<RoomUserChatEvent>(),
            
            [ClientPacketId.RoomUserShout] = provider.GetRequiredService<RoomUserShoutEvent>(),
            
            [ClientPacketId.RoomUserWalk] = new RoomUserWalkEvent(provider.GetRequiredService<IRoomRepository>()),
            [ClientPacketId.RoomUserDance] = new RoomUserDanceEvent(provider.GetRequiredService<IRoomRepository>()),
            [ClientPacketId.RoomUserAction] = new RoomUserActionEvent(provider.GetRequiredService<IRoomRepository>()),
            [ClientPacketId.RoomUserStartTyping] = new RoomUserStartTypingEvent(provider.GetRequiredService<IRoomRepository>()),
            [ClientPacketId.RoomUserStopTyping] = new RoomUserStopTypingEvent(provider.GetRequiredService<IRoomRepository>()),
            
            [ClientPacketId.RoomUserWhisper] = new RoomUserWhisperEvent(provider.GetRequiredService<IRoomRepository>(), 
                provider.GetRequiredService<RoomConstants>()),
            
            [ClientPacketId.RoomUserLookAt] = new RoomUserLookAtEvent(provider.GetRequiredService<IRoomRepository>()),
            [ClientPacketId.RoomUserSign] = new RoomUserSignEvent(provider.GetRequiredService<IRoomRepository>()),
            [ClientPacketId.RoomUserSit] = new RoomUserSitEvent(provider.GetRequiredService<IRoomRepository>()),
            [ClientPacketId.SaveNavigatorSettings] = new SaveNavigatorSettingsEvent(),
            
            [ClientPacketId.NavigatorSearch] = new NavigatorSearchEvent(
                provider.GetRequiredService<NavigatorTabRepository>(), 
                provider.GetRequiredService<NavigatorRoomProvider>()),
            
            [ClientPacketId.PlayerChangedAppearance] = new PlayerChangedAppearanceEvent(provider.GetRequiredService<IRoomRepository>()),
            [ClientPacketId.PlayerChangedMotto] = provider.GetRequiredService<PlayerChangedMottoEvent>(),
            [ClientPacketId.PlayerRelationships] = provider.GetRequiredService<PlayerRelationshipsEvent>(),
            [ClientPacketId.RoomUserTags] = provider.GetRequiredService<RoomUserTagsEvent>(),
            [ClientPacketId.RoomForwardData] = provider.GetRequiredService<RoomForwardDataEvent>(),
            [ClientPacketId.PlayerProfile] = provider.GetRequiredService<PlayerProfileEvent>(),
            [ClientPacketId.PlayerBadges] = provider.GetRequiredService<PlayerWearingBadgesEvent>(),
            
            [ClientPacketId.RoomUserRespect] = new RoomUserRespectEvent(
                provider.GetRequiredService<IPlayerRepository>(), 
                provider.GetRequiredService<IRoomRepository>(), 
                provider.GetRequiredService<IPlayerRespectDao>()),
            
            [ClientPacketId.PlayerFriendRequest] = new PlayerSendFriendRequestEvent(provider.GetRequiredService<IPlayerRepository>(), 
                
                provider.GetRequiredService<IPlayerFriendshipRepository>(), provider.GetRequiredService<PlayerConstants>()),
            [ClientPacketId.PlayerAcceptFriendRequest] = new PlayerAcceptFriendRequestEvent(provider.GetRequiredService<IPlayerRepository>(), 
                provider.GetRequiredService<IPlayerFriendshipRepository>(), provider.GetRequiredService<IRoomRepository>()),
            [ClientPacketId.PlayerDeclineFriendRequest] = new PlayerDeclineFriendRequestEvent(provider.GetRequiredService<IPlayerRepository>(), 
                provider.GetRequiredService<IPlayerFriendshipRepository>()),
            [ClientPacketId.PlayerRemoveFriend] = new PlayerRemoveFriendsEvent(provider.GetRequiredService<IPlayerRepository>(), 
                provider.GetRequiredService<IPlayerFriendshipRepository>()),
            [ClientPacketId.PlayerChangeRelation] = provider.GetRequiredService<PlayerChangeRelationEvent>(),
            [ClientPacketId.PlayerCreateRoom] = provider.GetRequiredService<PlayerCreateRoomEvent>(),
            [ClientPacketId.HabboClubData] = provider.GetRequiredService<HabboClubDataEvent>(),
            [ClientPacketId.HabboClubCenter] = provider.GetRequiredService<PlayerClubCenterDataEvent>(),
            [ClientPacketId.HabboClubGifts] = provider.GetRequiredService<HabboClubGiftsEvent>(),
            [ClientPacketId.RoomUserGoToHotelView] = provider.GetRequiredService<RoomUserGoToHotelViewEvent>(),
            [ClientPacketId.PlayerSearch] = provider.GetRequiredService<PlayerSearchEvent>(),
            [ClientPacketId.PlayerStalk] = provider.GetRequiredService<PlayerStalkEvent>(),
            [ClientPacketId.PlayerSendMessage] = provider.GetRequiredService<PlayerSendDirectMessageEvent>(),
            [ClientPacketId.RoomSettings] = provider.GetRequiredService<RoomSettingsEvent>(),
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
        });
    }
}