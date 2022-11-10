using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Game.Rooms.Packets.Writers;

public class PlayerHotelViewWriter : NetworkPacketWriter
{
    public PlayerHotelViewWriter()
    {
        WriteShort(ServerPacketId.RoomUserHotelView);
    }
}