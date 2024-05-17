using Sadie.Database;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Players;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Players.Rooms;

namespace Sadie.Networking.Events.Handlers.Players.Club;

[PacketId(EventHandlerIds.PlayerSetHomeRoom)]
public class PlayerSetHomeRoomEventHandler(
    PlayerSetHomeRoomEventParser parser,
    SadieContext dbContext) : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        parser.Parse(reader);

        if (client.Player?.NetworkObject == null)
        {
            return;
        }

        await client.Player.NetworkObject.WriteToStreamAsync(new PlayerHomeRoomWriter
        {
            HomeRoom = parser.RoomId,
            RoomIdToEnter = 0,
        });
        
        client.Player.Data.HomeRoomId = parser.RoomId;

        dbContext.PlayerData.Update(client.Player.Data);
        await dbContext.SaveChangesAsync();
    }
}