using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.GameCentre;

[PacketId(ServerPacketId.GameCentreConfig)]
public class GameAchievementsListWriter : AbstractPacketWriter
{
    public override void OnSerialize(NetworkPacketWriter writer)
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