using System.Drawing;
using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Rooms.Users;

namespace Sadie.Networking.Events.Rooms.Users;

public class RoomUserActionEvent : INetworkPacketEvent
{
    private readonly IRoomRepository _roomRepository;

    public RoomUserActionEvent(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        if (!PacketEventHelpers.TryResolveRoomObjectsForClient(_roomRepository, client, out var room, out var roomUser))
        {
            return;
        }
        
        var actionId = reader.ReadInt();
        await client.WriteToStreamAsync(new RoomUserActionWriter(roomUser!.Id, actionId).GetAllBytes());
    }
}