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
using Sadie.Game.Rooms;
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

            serviceCollection.AddSingleton<IRoomDao, RoomDao>();
            serviceCollection.AddSingleton(new ConcurrentDictionary<long, RoomEntity>());
            serviceCollection.AddSingleton<IRoomRepository, RoomRepository>();
            
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
                [ClientPacketId.GameCenterRequestGames] = new RequestGameCenterConfigEvent(), // CLEAN PACKET NAME
                [ClientPacketId.PromotedRooms] = new PromotedRoomsEvent(),
                [ClientPacketId.RoomCategories] = new RoomCategoriesEvent(),
                [ClientPacketId.NavigatorEventCategories] = new NavigatorEventCategoriesEvent(),
                [ClientPacketId.PlayerFriendRequestsList] = new PlayerFriendRequestsListEvent(),
                [ClientPacketId.PlayerSanctionStatus] = new PlayerSanctionStatusEvent(),
                [ClientPacketId.UnknownEvent2] = new UnknownEvent2(),
                [ClientPacketId.RoomLoaded] = new RoomLoadedEvent(provider.GetRequiredService<IRoomRepository>()),
                [ClientPacketId.UnknownEvent3] = new UnknownEvent3(),
                [ClientPacketId.RoomHeightmap] = new RoomHeightmapEvent(),
            });
            
            serviceCollection.AddSingleton<INetworkPacketHandler, ClientPacketHandler>();
            serviceCollection.AddSingleton<INetworkClientFactory, NetworkClientFactory>();
            serviceCollection.AddSingleton<INetworkClientRepository, NetworkClientRepository>();
            serviceCollection.AddTransient<INetworkClient, NetworkClient>();
            serviceCollection.AddSingleton<INetworkListener, NetworkListener>();
        }
    }
}