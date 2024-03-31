using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Rooms.Users;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Handlers.Rooms.Users;

public class RoomUserSignEventHandler(RoomUserSignEventParser eventParser, IRoomRepository roomRepository) : INetworkPacketEventHandler
{
    public Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);

        if (!PacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out var room, out var roomUser))
        {
            return Task.CompletedTask;
        }

        roomUser!.StatusMap[RoomUserStatus.Sign] = eventParser.SignId.ToString();
        
        return Task.CompletedTask;
    }
}