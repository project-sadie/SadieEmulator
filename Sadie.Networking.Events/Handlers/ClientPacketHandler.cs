using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.API.Interfaces.Networking.Packets;
using Sadie.Networking.Events.Attributes;
using Sadie.Networking.Events.Handlers.Rooms.Users;
using Sadie.Networking.Events.Handlers.Rooms.Users.Chat;
using Sadie.Networking.Options;
using Sadie.Networking.Packets;
using Sadie.Networking.Packets.Writers.Generic;

namespace Sadie.Networking.Events.Handlers;

public class ClientPacketHandler(
    ILogger<ClientPacketHandler> logger,
    Dictionary<short, Type> packetHandlerTypeMap,
    PacketHandlerFactory handlerFactory,
    IOptions<NetworkPacketOptions> packetOptions)
    : INetworkPacketHandler
{
    public async Task HandleAsync(INetworkClient client, INetworkPacket packet)
    {
        try
        {
            if (!packetHandlerTypeMap.TryGetValue(packet.PacketId, out var packetEventType))
            {
                if (packetOptions.Value.NotifyMissingPacket)
                {
                    _ = NotifyMissingPacketAsync(packet.PacketId, client);
                }
            
                logger.LogWarning($"Couldn't resolve packet event handler for header '{packet.PacketId}'");
                return;
            }

            var eventHandler = handlerFactory.Create(packet.PacketId);

            if (!ValidateAttributes(eventHandler, client))
            {
                return;
            }
            
            var packetReader = new NetworkPacketReader(packet.Data);
            EventSerializer.SetPropertiesForEventHandler(eventHandler, packetReader);

            if (client.RoomUser != null &&
                (packetEventType == typeof(RoomUserWalkEventHandler) ||
                 packetEventType == typeof(RoomUserChatEventHandler) ||
                 packetEventType == typeof(RoomUserShoutEventHandler) ||
                 packetEventType == typeof(RoomUserActionEventHandler) ||
                 packetEventType == typeof(RoomUserDanceEventHandler) ||
                 packetEventType == typeof(RoomUserSignEventHandler) ||
                 packetEventType == typeof(RoomUserSitEventHandler) ||
                 packetEventType == typeof(RoomUserLookAtEventHandler)))
            {
                client.RoomUser.LastAction = DateTime.Now;
            }

            await ExecuteAsync(client, eventHandler);
        }
        catch (Exception e)
        {
            logger.LogCritical(e.ToString());
        }
    }

    private async Task NotifyMissingPacketAsync(int messageId, INetworkClient client)
    {
        try
        {
            var writer = new ServerErrorWriter
            {
                MessageId = messageId,
                ErrorCode = 1,
                DateTime = DateTime.Now.ToString("M/d/yy, h:mm tt")
            };

            await client.WriteToStreamAsync(writer);
        }
        catch (Exception e)
        {
            logger.LogCritical(e.ToString());
        }
    }

    private static bool ValidateAttributes(INetworkPacketEventHandler eventHandler, 
        INetworkClient client)
    {
        var method = eventHandler.GetType().GetMethods()
            .SingleOrDefault(x => x.Name == "HandleAsync");

        var requiresRoomRights = method?.GetCustomAttributes(typeof(RequiresRoomRightsAttribute), true)
            .FirstOrDefault() != null;

        if (requiresRoomRights)
        {
            return client.RoomUser != null && client.RoomUser.HasRights();
        }

        return true;
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