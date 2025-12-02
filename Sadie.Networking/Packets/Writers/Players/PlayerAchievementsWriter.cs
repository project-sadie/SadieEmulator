using Sadie.API;
using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Packets.Writers.Players;

[PacketId(ServerPacketId.PlayerAchievements)]
public class PlayerAchievementsWriter : AbstractPacketWriter
{
    public override async Task OnSerializeAsync(INetworkPacketWriter writer)
    {
        writer.WriteInteger(0);
        writer.WriteInteger(0);
    }
}