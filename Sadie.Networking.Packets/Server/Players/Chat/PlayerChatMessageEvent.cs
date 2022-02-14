using Sadie.Networking.Client;

namespace Sadie.Networking.Packets.Server.Players.Chat;

public class PlayerChatMessageEvent : INetworkPacketEvent
{
    public Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var message = reader.ReadString();

        if (message.Length > 100)
        {
            return Task.CompletedTask;
        }
        
        var bubbleColor = reader.ReadInt();
        
        return Task.CompletedTask;
    }
}