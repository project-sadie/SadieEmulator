using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Game.Rooms.Packets;

public class PlayerHotelViewWriter : NetworkPacketWriter
{
    public PlayerHotelViewWriter()
    {
        WriteShort(ServerPacketId.RoomUserHotelView);
    }
}