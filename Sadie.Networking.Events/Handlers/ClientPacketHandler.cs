using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sadie.Networking.Client;
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
            logger.LogWarning($"Couldn't resolve packet eventHandler for header '{packet.PacketId}'");
            return;
        }

        var parameters = GetParametersForHandler(packetEventType, packet);
        var eventHandler = (INetworkPacketEventHandler) ActivatorUtilities.CreateInstance(serviceProvider, packetEventType, parameters);
        await ExecuteAsync(client, packet, eventHandler);
    }

    private object[] GetParametersForHandler(Type packetEventType, INetworkPacket packet)
    {
        var parameters = new List<object>();
        
        foreach (var property in packetEventType.GetProperties())
        {
            var type = property.PropertyType;

            if (type == typeof(int))
            {
                parameters.Add(packet.ReadInt());
            }
            else if (type == typeof(string))
            {
                parameters.Add(packet.ReadString());
            }
            else if (type == typeof(bool))
            {
                parameters.Add(packet.ReadBool());
            }
            else
            {
                throw new Exception($"{type.FullName}");
            }
        }

        return parameters.ToArray();
    }

    private async Task ExecuteAsync(INetworkClient client, INetworkPacketReader packet, INetworkPacketEventHandler eventHandler)
    {
        logger.LogDebug($"Executing packet '{eventHandler.GetType().Name}'");
        
        try
        {
            await eventHandler.HandleAsync(client, packet);
        }
        catch (Exception e)
        {
            logger.LogError(e.ToString());
        }
    }
}