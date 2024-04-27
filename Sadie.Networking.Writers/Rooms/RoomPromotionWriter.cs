using Sadie.Networking.Serialization;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets.Attributes;

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