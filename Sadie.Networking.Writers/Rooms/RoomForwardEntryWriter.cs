using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms;

public class RoomForwardEntryWriter : NetworkPacketWriter
{
    public RoomForwardEntryWriter(int roomId)
    {
        WriteShort(ServerPacketId.RoomForwardEntry);
        WriteInteger(roomId);
    }
}