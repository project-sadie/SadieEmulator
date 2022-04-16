using Sadie.Game.Rooms;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

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