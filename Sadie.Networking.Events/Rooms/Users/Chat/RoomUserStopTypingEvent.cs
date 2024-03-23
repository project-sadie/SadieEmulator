using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Rooms.Users.Chat;

namespace Sadie.Networking.Events.Rooms.Users.Chat;

public class RoomUserStopTypingEvent(IRoomRepository roomRepository) : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        if (!PacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out var room, out var roomUser))
        {
            return;
        }

        roomUser.UpdateLastAction();
        
        await room.UserRepository.BroadcastDataAsync(new RoomUserTypingWriter(roomUser.Id, false).GetAllBytes());
    }
}