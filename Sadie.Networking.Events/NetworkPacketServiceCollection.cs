using Microsoft.Extensions.DependencyInjection;
using Sadie.Networking.Encryption;
using Sadie.Networking.Events.Handlers;
using Sadie.Networking.Events.Handlers.Rooms;
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
            .AsImplementedInterfaces()
            .WithSingletonLifetime());

        serviceCollection.AddSingleton<HabboEncryption>();
        serviceCollection.AddSingleton<RoomHeightmapEventHandler>();
        
        serviceCollection.AddSingleton<INetworkPacketHandler, ClientPacketHandler>();
    }
}