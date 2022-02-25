using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sadie.Game.Players;
using Sadie.Game.Players.Friendships;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Categories;
using Sadie.Networking.Events.GameCenter;
using Sadie.Networking.Events.Handshake;
using Sadie.Networking.Events.HotelView;
using Sadie.Networking.Events.Navigator;
using Sadie.Networking.Events.Players;
using Sadie.Networking.Events.Players.Club;
using Sadie.Networking.Events.Players.Friends;
using Sadie.Networking.Events.Rooms;
using Sadie.Networking.Events.Rooms.Users;
using Sadie.Networking.Events.Rooms.Users.Chat;
using Sadie.Networking.Events.Tracking;
using Sadie.Networking.Events.Unknown;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events;

public class NetworkPacketServiceCollection
{
    public static void AddServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<INetworkPacketHandler, ClientPacketHandler>();
        
        serviceCollection.AddSingleton(provider => new ConcurrentDictionary<int, INetworkPacketEvent>
        {
            [ClientPacketId.ClientVersion] = new ClientVersionEvent(),
            [ClientPacketId.ClientVariables] = new ClientVariablesEvent(),
            [ClientPacketId.MachineId] = new MachineIdEvent(),
            [ClientPacketId.SecureLogin] = new SecureLoginEvent(provider.GetRequiredService<ILogger<SecureLoginEvent>>(), provider.GetRequiredService<IPlayerRepository>()),
            [ClientPacketId.PerformanceLog] = new PerformanceLogEvent(),
            [ClientPacketId.PlayerActivity] = new PlayerActivityEvent(provider.GetRequiredService<ILogger<PlayerActivityEvent>>(), provider.GetRequiredService<IPlayerRepository>()),
            [ClientPacketId.PlayerData] = new PlayerDataEvent(),
            [ClientPacketId.PlayerBalance] = new PlayerBalanceEvent(),
            [ClientPacketId.PlayerClubMembership] = new PlayerClubMembershipEvent(),
            [ClientPacketId.NavigatorData] = new NavigatorDataEvent(),
            [ClientPacketId.PlayerFriendsList] = new PlayerFriendsListEvent(),
            [ClientPacketId.PlayerMessengerInit] = new PlayerMessengerInitEvent(),
            [ClientPacketId.PlayerPing] = new PlayerPingEvent(),
            [ClientPacketId.HotelViewData] = new HotelViewDataEvent(),
            [ClientPacketId.PlayerUsername] = new PlayerUsernameEvent(),
            [ClientPacketId.PlayerMeMenuSettings] = new PlayerMeMenuSettingsEvent(),
            [ClientPacketId.HotelViewBonusRare] = new HotelViewBonusRareEvent(),
            [ClientPacketId.UnknownEvent1] = new UnknownEvent1(),
            [ClientPacketId.GameCenterRequestGames] = new RequestGameCenterConfigEvent(),
            [ClientPacketId.PromotedRooms] = new PromotedRoomsEvent(),
            [ClientPacketId.RoomCategories] = new RoomCategoriesEvent(provider.GetRequiredService<IRoomCategoryRepository>()),
            [ClientPacketId.NavigatorEventCategories] = new NavigatorEventCategoriesEvent(),
            [ClientPacketId.PlayerFriendRequestsList] = new PlayerFriendRequestsListEvent(provider.GetRequiredService<IPlayerFriendshipRepository>()),
            [ClientPacketId.PlayerSanctionStatus] = new PlayerSanctionStatusEvent(),
            [ClientPacketId.UnknownEvent2] = new UnknownEvent2(),
            [ClientPacketId.RoomLoaded] = ActivatorUtilities.CreateInstance<RoomLoadedEvent>(provider),
            [ClientPacketId.UnknownEvent3] = new UnknownEvent3(),
            [ClientPacketId.RoomHeightmap] = new RoomHeightmapEvent(provider.GetRequiredService<IRoomRepository>()),
            [ClientPacketId.RoomUserChat] = new RoomUserChatEvent(provider.GetRequiredService<IRoomRepository>()),
            [ClientPacketId.RoomUserShout] = new RoomUserShoutEvent(provider.GetRequiredService<IRoomRepository>()),
            [ClientPacketId.RoomUserWalk] = new RoomUserWalkEvent(provider.GetRequiredService<IRoomRepository>())
        });
    }
}