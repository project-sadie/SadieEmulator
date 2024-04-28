using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Rooms;

[PacketId(ServerPacketId.RoomPromotion)]
public class RoomPromotionWriter : AbstractPacketWriter
{
    public override void OnSerialize(NetworkPacketWriter writer)
    {
        writer.WriteInteger(-1);
        writer.WriteInteger(-1);
        writer.WriteString("");
        writer.WriteInteger(0);
        writer.WriteInteger(0);
        writer.WriteString("");
        writer.WriteString("");
        writer.WriteInteger(0);
        writer.WriteInteger(0);
        writer.WriteInteger(0);
    }
}