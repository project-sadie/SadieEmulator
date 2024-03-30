using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms;

public class RoomPaneWriter : NetworkPacketWriter
{
    public RoomPaneWriter(int roomId, bool owner)
    {
        WriteShort(ServerPacketId.RoomPane);
        WriteInteger(roomId);
        WriteBool(owner);
    }
}