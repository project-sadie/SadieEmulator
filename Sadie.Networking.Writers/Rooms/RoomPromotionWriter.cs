using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms;

public class RoomPromotionWriter : AbstractPacketWriter
{
    public RoomPromotionWriter()
    {
        WriteShort(ServerPacketId.RoomPromotion);
        WriteInteger(-1);
        WriteInteger(-1);
        WriteString("");
        WriteInteger(0);
        WriteInteger(0);
        WriteString("");
        WriteString("");
        WriteInteger(0);
        WriteInteger(0);
        WriteInteger(0);
    }
}