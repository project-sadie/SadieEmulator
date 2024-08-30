using Sadie.API;
using Sadie.API.Networking;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.GameCentre;

[PacketId(ServerPacketId.GameCentreConfig)]
public class GameAchievementsListWriter : AbstractPacketWriter
{
    public override void OnSerialize(INetworkPacketWriter writer)
    {
        writer.WriteInteger(0);
        writer.WriteInteger(0);
        writer.WriteInteger(0);
        writer.WriteInteger(0);
        writer.WriteInteger(0);
        writer.WriteInteger(0);
        writer.WriteInteger(0);
        writer.WriteInteger(0);
        writer.WriteInteger(0);
        writer.WriteInteger(0);
        writer.WriteInteger(3);
        writer.WriteInteger(1);
        writer.WriteInteger(1);
        writer.WriteString("BaseJumpBigParachute");
        writer.WriteInteger(1);
    }
}