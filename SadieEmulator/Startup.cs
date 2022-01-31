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
                [ClientPacketIds.ReceiveClientVersion] = new ReceivedClientVersionEvent(),
                [ClientPacketIds.ReceiveClientVariables] = new ReceivedClientVariablesEvent(),
                [ClientPacketIds.ReceivedUniqueMachineId] = new ReceivedMachineIdEvent(),
                [ClientPacketIds.TrySecureLogin] = new RequestSecureLoginEvent(provider.GetRequiredService<ILogger<RequestSecureLoginEvent>>(), provider.GetRequiredService<IPlayerRepository>()),
                [ClientPacketIds.PerformanceLog] = new PerformanceLogPacket(),
                [ClientPacketIds.PlayerActivity] = new PlayerActivityEvent(provider.GetRequiredService<ILogger<PlayerActivityEvent>>(), provider.GetRequiredService<IPlayerRepository>()),
                [ClientPacketIds.PlayerData] = new PlayerDataEvent(),
                [ClientPacketIds.RequestPlayerBalance] = new RequestPlayerBalancePacket(),
                [ClientPacketIds.RequestPlayerClubMembership] = new RequestPlayerClubMembership(),
                [ClientPacketIds.RequestNewNavigatorData] = new RequestNewNavigatorData(),
                [ClientPacketIds.RequestFriendsEvent] = new RequestFriendsEvent(),
                [ClientPacketIds.RequestInitFriendsEvent] = new RequestInitFriendsEvent(),
                [ClientPacketIds.PingEvent] = new PlayerPingEvent(),
                [ClientPacketIds.HotelViewData] = new RequestHotelViewDataEvent(),
                [ClientPacketIds.PlayerUsername] = new PlayerUsernameEvent(),
                [ClientPacketIds.PlayerMeMenuSettings] = new PlayerMeMenuSettingsEvent(),
                [ClientPacketIds.HotelViewBonusRare] = new RequestHotelViewBonusRareEvent(),
                [ClientPacketIds.UnknownEvent1] = new UnknownEvent1(),
                [ClientPacketIds.GameCenterRequestGames] = new RequestGameCenterConfigEvent(),
                [ClientPacketIds.RequestPromotedRooms] = new RequestPromotedRoomsEvent(),
                [ClientPacketIds.RequestRoomCategories] = new RequestRoomCategoriesEvent(),
                [ClientPacketIds.GetEventCategories] = new GetEventCategoriesMessageEvent(),
                [ClientPacketIds.RequestFriendRequest] = new RequestFriendRequestEvent(),
                [ClientPacketIds.PlayerSanctionStatus] = new PlayerSanctionStatusEvent(),
                [ClientPacketIds.RequestTargetOffer] = new UnknownEvent2(),
            });
            
            serviceCollection.AddSingleton<INetworkPacketHandler, ClientPacketHandler>();
            serviceCollection.AddSingleton<INetworkClientFactory, NetworkClientFactory>();
            serviceCollection.AddSingleton<INetworkClientRepository, NetworkClientRepository>();
            serviceCollection.AddTransient<INetworkClient, NetworkClient>();
            serviceCollection.AddSingleton<INetworkListener, NetworkListener>();
        }
    }
}