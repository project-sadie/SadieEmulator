using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Players.Clothing;

[PacketId(ServerPacketId.PlayerClothingList)]
public class PlayerClothingListWriter : AbstractPacketWriter
{
    public override void OnSerialize(NetworkPacketWriter writer)
    {
        writer.WriteInteger(0);
        writer.WriteInteger(0);
    }
}