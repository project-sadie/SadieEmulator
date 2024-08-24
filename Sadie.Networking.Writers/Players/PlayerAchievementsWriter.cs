using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Players;

[PacketId(ServerPacketId.PlayerAchievements)]
public class PlayerAchievementsWriter : AbstractPacketWriter
{
    public override void OnSerialize(NetworkPacketWriter writer)
    {
        writer.WriteInteger(0);
        writer.WriteInteger(0);
    }
}