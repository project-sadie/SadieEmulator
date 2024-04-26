using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms;

public class RoomPaneWriter : AbstractPacketWriter
{
    public RoomPaneWriter(int roomId, bool owner)
    {
        WriteShort(ServerPacketId.RoomPane);
        WriteInteger(roomId);
        WriteBool(owner);
    }
}