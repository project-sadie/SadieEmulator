using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Rooms.Users;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Rooms.Users;
using Sadie.Shared.Unsorted;

namespace Sadie.Networking.Events.Handlers.Rooms.Users;

public class RoomUserActionEventHandler(RoomUserActionEventParser eventParser, RoomRepository roomRepository) : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.RoomUserAction;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);

        if (!NetworkPacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out var room, out var roomUser))
        {
            return;
        }

        if (eventParser.Action == RoomUserAction.Idle)
        {
            if (!roomUser.IsIdle)
            {
                roomUser.LastAction -= roomUser.IdleTime;
            }
            
            await room!.UserRepository.BroadcastDataAsync(new RoomUserIdleWriter(roomUser.Id, roomUser.IsIdle).GetAllBytes());
            return;
        }
        
        roomUser.UpdateLastAction();

        await room!.UserRepository.BroadcastDataAsync(new RoomUserActionWriter(roomUser.Id, (int) eventParser.Action).GetAllBytes());
    }
}