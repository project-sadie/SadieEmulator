using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;
using Sadie.Shared.Unsorted.Networking.Packets.Attributes;

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