using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Handlers.Rooms.Users;
using Sadie.Networking.Events.Handlers.Rooms.Users.Chat;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Generic;

namespace Sadie.Networking.Events.Handlers;

public class ClientPacketHandler(
    ILogger<ClientPacketHandler> logger,
    Dictionary<short, Type> packetHandlerTypeMap,
    IServiceProvider serviceProvider)
    : INetworkPacketHandler
{
    public async Task HandleAsync(INetworkClient client, INetworkPacket packet)
    {
        if (!packetHandlerTypeMap.TryGetValue(packet.PacketId, out var packetEventType))
        {
            var writer = new ServerErrorWriter
            {
                MessageId = packet.PacketId,
                ErrorCode = 1,
                DateTime = DateTime.Now.ToString("M/d/yy, h:mm tt")
            };
                
            await client.WriteToStreamAsync(writer);
            logger.LogWarning($"Couldn't resolve packet event handler for header '{packet.PacketId}'");
            return;
        }

        var eventHandler = (INetworkPacketEventHandler) ActivatorUtilities.CreateInstance(serviceProvider, packetEventType);
        EventSerializer.SetPropertiesForEventHandler(eventHandler, packet);

        if (client.RoomUser != null && 
            (packetEventType == typeof(RoomUserWalkEventHandler) || 
             packetEventType == typeof(RoomUserChatEventHandler) ||
             packetEventType == typeof(RoomUserShoutEventHandler) ||
             packetEventType == typeof(RoomUserActionEventHandler) ||
             packetEventType == typeof(RoomUserDanceEventHandler) ||
             packetEventType == typeof(RoomUserSignEventHandler) ||
             packetEventType == typeof(RoomUserSitEventHandler)))
        {
            client.RoomUser.LastAction = DateTime.Now;
        }
        
        await ExecuteAsync(client, eventHandler);
    }

    private async Task ExecuteAsync(INetworkClient client, INetworkPacketEventHandler eventHandler)
    {
        logger.LogDebug($"Executing packet '{eventHandler.GetType().Name}'");
        
        try
        {
            await eventHandler.HandleAsync(client);
        }
        catch (Exception e)
        {
            logger.LogError(e.ToString());
        }
    }
}