using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms;

public class RoomForwardEntryWriter : NetworkPacketWriter
{
    public RoomForwardEntryWriter(int roomId)
    {
        WriteShort(ServerPacketId.RoomForwardEntry);
        WriteInteger(roomId);
    }
}