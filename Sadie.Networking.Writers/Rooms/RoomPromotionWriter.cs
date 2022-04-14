using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms;

public class RoomPromotionWriter : NetworkPacketWriter
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