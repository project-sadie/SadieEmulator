using Sadie.Game.Rooms;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms;

public class RoomRightsWriter : NetworkPacketWriter
{
    public RoomRightsWriter(RoomControllerLevel controllerLevel)
    {
        WriteShort(ServerPacketId.RoomRights);
        WriteInteger((int) controllerLevel);
    }
}