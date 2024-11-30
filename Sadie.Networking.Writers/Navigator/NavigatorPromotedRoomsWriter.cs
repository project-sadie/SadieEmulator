using Sadie.API;
using Sadie.API.Networking;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Navigator;

[PacketId(ServerPacketId.NavigatorPromotedRooms)]
public class NavigatorPromotedRoomsWriter : AbstractPacketWriter
{
    public override void OnSerialize(INetworkPacketWriter writer)
    {
        writer.WriteInteger(2);
        writer.WriteString("");
        writer.WriteInteger(0);
        writer.WriteBool(true);
        writer.WriteInteger(0);
        writer.WriteString("A");
        writer.WriteString("B");
        writer.WriteInteger(1);
        writer.WriteString("C");
        writer.WriteString("D");
        writer.WriteInteger(1);
        writer.WriteInteger(1);
        writer.WriteInteger(1);
        writer.WriteString("E");
    }
}