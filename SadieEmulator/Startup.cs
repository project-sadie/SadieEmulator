using System.Collections.Concurrent;
using System.Data.Common;
using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using Sadie.Database;
using Sadie.Game.Players;
using Sadie.Networking;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Packets.Client;
using Sadie.Networking.Packets.Client.GameCenter;
using Sadie.Networking.Packets.Client.Handshake;
using Sadie.Networking.Packets.Client.HotelView;
using Sadie.Networking.Packets.Client.Navigator;
using Sadie.Networking.Packets.Client.Players;
using Sadie.Networking.Packets.Client.Players.Club;
using Sadie.Networking.Packets.Client.Players.Friends;
using Sadie.Networking.Packets.Client.Rooms;
using Sadie.Networking.Packets.Client.Tracking;
using Sadie.Networking.Packets.Client.Unknown;

namespace SadieEmulator
{
    public static class Startup
    {
        public static void ConfigureServices(HostBuilderContext hostBuilderContext, IServiceCollection serviceCollection)
        {
            var config = serviceCollection.BuildServiceProvider().GetRequiredService<IConfiguration>();
            
            ConfigureServer(serviceCollection);
            ConfigureDatabase(config, serviceCollection);

            serviceCollection.AddSingleton<IPlayerDao, PlayerDao>();
            serviceCollection.AddSingleton<IPlayerRepository, PlayerRepository>();
            
            ConfigureNetworking(config, serviceCollection);
        }

        private static void ConfigureServer(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IServer, Server>();
        }

        private static void ConfigureDatabase(IConfiguration config, IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IDatabaseConnection, DatabaseConnection>();
            serviceCollection.AddSingleton<DbConnectionStringBuilder>(new MySqlConnectionStringBuilder(config.GetConnectionString("Default")));
            serviceCollection.AddSingleton<IDatabaseProvider, DatabaseProvider>();
        }

        private static void ConfigureNetworking(IConfiguration config, IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton(new TcpListener(
                IPAddress.Parse(config.GetValue<string>("Networking:Host")), 
                config.GetValue<int>("Networking:Port"))  
            );

            serviceCollection.AddSingleton(provider => new ConcurrentDictionary<int, INetworkPacketEvent>
            {
                [ClientPacketId.ReceiveClientVersion] = new ClientVersionEvent(),
                [ClientPacketId.ReceiveClientVariables] = new ClientVariablesEvent(),
                [ClientPacketId.ReceivedUniqueMachineId] = new MachineIdEvent(),
                [ClientPacketId.TrySecureLogin] = new SecureLoginEvent(provider.GetRequiredService<ILogger<SecureLoginEvent>>(), provider.GetRequiredService<IPlayerRepository>()),
                [ClientPacketId.PerformanceLog] = new PerformanceLogEvent(),
                [ClientPacketId.PlayerActivity] = new PlayerActivityEvent(provider.GetRequiredService<ILogger<PlayerActivityEvent>>(), provider.GetRequiredService<IPlayerRepository>()),
                [ClientPacketId.PlayerData] = new PlayerDataEvent(),
                [ClientPacketId.RequestPlayerBalance] = new PlayerBalanceEvent(),
                [ClientPacketId.RequestPlayerClubMembership] = new PlayerClubMembershipEvent(),
                [ClientPacketId.RequestNewNavigatorData] = new NavigatorDataEvent(),
                [ClientPacketId.RequestFriendsEvent] = new PlayerFriendsListEvent(),
                [ClientPacketId.RequestInitFriendsEvent] = new PlayerMessengerInitEvent(),
                [ClientPacketId.PingEvent] = new PlayerPingEvent(),
                [ClientPacketId.HotelViewData] = new HotelViewDataEvent(),
                [ClientPacketId.PlayerUsername] = new PlayerUsernameEvent(),
                [ClientPacketId.PlayerMeMenuSettings] = new PlayerMeMenuSettingsEvent(),
                [ClientPacketId.HotelViewBonusRare] = new HotelViewBonusRareEvent(),
                [ClientPacketId.UnknownEvent1] = new UnknownEvent1(),
                [ClientPacketId.GameCenterRequestGames] = new RequestGameCenterConfigEvent(),
                [ClientPacketId.RequestPromotedRooms] = new PromotedRoomsEvent(),
                [ClientPacketId.RequestRoomCategories] = new RoomCategoriesEvent(),
                [ClientPacketId.GetEventCategories] = new NavigatorEventCategoriesMessageEvent(),
                [ClientPacketId.RequestFriendRequest] = new PlayerFriendRequestsListEvent(),
                [ClientPacketId.PlayerSanctionStatus] = new PlayerSanctionStatusEvent(),
                [ClientPacketId.RequestTargetOffer] = new UnknownEvent2(),
                [ClientPacketId.LoadRoom] = new RoomLoadedEvent(),
                [ClientPacketId.UnknownEvent3] = new UnknownEvent3(),
                [ClientPacketId.GetRoomHeightmap] = new RoomHeightmapEvent(),
                // 21
                // 796
                // 219
                // 3320
            });
            
            serviceCollection.AddSingleton<INetworkPacketHandler, ClientPacketHandler>();
            serviceCollection.AddSingleton<INetworkClientFactory, NetworkClientFactory>();
            serviceCollection.AddSingleton<INetworkClientRepository, NetworkClientRepository>();
            serviceCollection.AddTransient<INetworkClient, NetworkClient>();
            serviceCollection.AddSingleton<INetworkListener, NetworkListener>();
        }
    }
}