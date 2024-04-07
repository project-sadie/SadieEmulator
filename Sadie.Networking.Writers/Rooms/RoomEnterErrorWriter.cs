using Sadie.Game.Rooms.Enums;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms;

public class RoomEnterErrorWriter : NetworkPacketWriter
{
    public RoomEnterErrorWriter(RoomEnterError errorCode)
    {
        WriteShort(ServerPacketId.RoomEnterError);
        WriteInteger((int) errorCode);
        WriteString("");
    }
}