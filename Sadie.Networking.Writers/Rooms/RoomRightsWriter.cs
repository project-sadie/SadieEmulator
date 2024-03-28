using Sadie.Game.Rooms;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms;

public class RoomRightsWriter : NetworkPacketWriter
{
    public RoomRightsWriter(RoomControllerLevel controllerLevel)
    {
        WriteShort(ServerPacketId.RoomRights);
        WriteInteger((int) controllerLevel);
    }
}