using Sadie.Database;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Players;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players.Rooms;

namespace Sadie.Networking.Events.Handlers.Players.Club;

public class PlayerSetHomeRoomEventHandler(
    PlayerSetHomeRoomEventParser parser,
    SadieContext dbContext) : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.PlayerSetHomeRoom;
    
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        parser.Parse(reader);

        if (client.Player == null || client.Player.NetworkObject == null)
        {
            return;
        }

        await client.Player.NetworkObject.WriteToStreamAsync(new PlayerHomeRoomWriter(parser.RoomId, parser.RoomId));
        
        client.Player.Data.HomeRoomId = parser.RoomId;

        dbContext.PlayerData.Update(client.Player.Data);
        await dbContext.SaveChangesAsync();
    }
}