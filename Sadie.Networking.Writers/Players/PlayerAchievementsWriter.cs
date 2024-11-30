using Sadie.API;
using Sadie.API.Networking;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Players;

[PacketId(ServerPacketId.PlayerAchievements)]
public class PlayerAchievementsWriter : AbstractPacketWriter
{
    public override void OnSerialize(INetworkPacketWriter writer)
    {
        writer.WriteInteger(0);
        writer.WriteInteger(0);
    }
}