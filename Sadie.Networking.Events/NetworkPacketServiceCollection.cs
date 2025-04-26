using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Sadie.Networking.Events.Handlers;
using Sadie.Networking.Events.Handlers.Rooms;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Events;

public static class NetworkPacketServiceCollection
{
    public static void AddServices(IServiceCollection serviceCollection)
    {
        serviceCollection.Scan(scan => scan
            .FromAssemblyOf<INetworkPacketEventHandler>()
            .AddClasses(classes => classes.AssignableTo<INetworkPacketEventHandler>())
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        var packetHandlerTypeMap = new Dictionary<short, Type>();
        
        foreach(var type in Assembly.GetExecutingAssembly().GetTypes())
        {
            var attributes = type.GetCustomAttributes(typeof(PacketIdAttribute), false);
            var headerAttribute = attributes.FirstOrDefault();

            if (headerAttribute == null)
            {
                continue;
            }
            
            packetHandlerTypeMap[((PacketIdAttribute) headerAttribute).Id] = type;
        }

        serviceCollection.AddSingleton(packetHandlerTypeMap);
        serviceCollection.AddSingleton<RoomHeightmapEventHandler>();
        serviceCollection.AddSingleton<INetworkPacketHandler, ClientPacketHandler>();
    }
}