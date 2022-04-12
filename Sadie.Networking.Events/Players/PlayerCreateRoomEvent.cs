using Sadie.Networking.Client;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Players;

public class PlayerCreateRoomEvent : INetworkPacketEvent
{
    public Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var name = reader.ReadString();
        var description = reader.ReadString();
        var modelName = reader.ReadString();
        var categoryId = reader.ReadInt();
        var maxUsersAllowed = reader.ReadInt();
        var tradingPermission = reader.ReadInt();
        
        
    }
}