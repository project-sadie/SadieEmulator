using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Game.Rooms.Packets.Writers;

public class RoomUserHandItemWriter : AbstractPacketWriter
{
    public RoomUserHandItemWriter(int userId, int itemId)
    {
        WriteShort(ServerPacketId.RoomUserHandItem);
        WriteInteger(userId);
        WriteInteger(itemId);
    }
}