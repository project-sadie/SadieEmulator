using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Game.Rooms.Packets;

public class RoomUserHotelViewWriter : NetworkPacketWriter
{
    public RoomUserHotelViewWriter()
    {
        WriteShort(ServerPacketId.RoomUserHotelView);
    }
}