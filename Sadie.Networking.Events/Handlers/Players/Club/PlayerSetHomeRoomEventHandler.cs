using Sadie.Database;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Players.Rooms;

namespace Sadie.Networking.Events.Handlers.Players.Club;

[PacketId(EventHandlerIds.PlayerSetHomeRoom)]
public class PlayerSetHomeRoomEventHandler(
    SadieContext dbContext) : INetworkPacketEventHandler
{
    public int RoomId { get; set; }
    
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        if (client.Player?.NetworkObject == null ||
            client.Player.Data.HomeRoomId == RoomId)
        {
            return;
        }

        await client.Player.NetworkObject.WriteToStreamAsync(new PlayerHomeRoomWriter
        {
            HomeRoom = RoomId,
            RoomIdToEnter = 0,
        });
        
        client.Player.Data.HomeRoomId = RoomId;

        dbContext.PlayerData.Update(client.Player.Data);
        await dbContext.SaveChangesAsync();
    }
}