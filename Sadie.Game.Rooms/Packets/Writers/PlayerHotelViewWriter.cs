using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Game.Rooms.Packets.Writers;

public class PlayerHotelViewWriter : NetworkPacketWriter
{
    public PlayerHotelViewWriter()
    {
        WriteShort(ServerPacketId.RoomUserHotelView);
    }
}