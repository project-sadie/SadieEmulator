using Sadie.API;
using Sadie.API.Networking;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Players.Clothing;

[PacketId(ServerPacketId.PlayerClothingList)]
public class PlayerClothingListWriter : AbstractPacketWriter
{
    public override void OnSerialize(INetworkPacketWriter writer)
    {
        writer.WriteInteger(0);
        writer.WriteInteger(0);
    }
}