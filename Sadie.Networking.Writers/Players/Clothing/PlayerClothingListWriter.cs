using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Players.Clothing;

[PacketId(ServerPacketId.PlayerClothingList)]
public class PlayerClothingListWriter : AbstractPacketWriter
{
    public override void OnSerialize(NetworkPacketWriter writer)
    {
        writer.WriteShort(ServerPacketId.PlayerClothingList);
        writer.WriteInteger(0);
        writer.WriteInteger(0);
    }
}