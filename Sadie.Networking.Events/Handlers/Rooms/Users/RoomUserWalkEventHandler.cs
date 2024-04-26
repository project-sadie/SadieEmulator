using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Rooms.Users;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Handlers.Rooms.Users;

public class RoomUserWalkEventHandler(RoomUserWalkEventParser eventParser, RoomRepository roomRepository) : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.RoomUserWalk;

    public Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);

        if (!NetworkPacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out _, out var roomUser))
        {
            return Task.CompletedTask;
        }
        
        roomUser.LastAction = DateTime.Now;
        
        roomUser.WalkToPoint(eventParser.Point);
        return Task.CompletedTask;
    }
}