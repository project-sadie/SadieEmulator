using Sadie.Game.Players.Badges;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Players;

public class PlayerBadgesWriter : NetworkPacketWriter
{
    public PlayerBadgesWriter(int playerId, List<PlayerBadge> badges)
    {
        WriteShort(ServerPacketId.PlayerBadges);
        WriteInt(playerId);
        WriteInt(badges.Count);

        foreach (var item in badges)
        {
            WriteInt(item.Slot);
            WriteString(item.Code);
        }
    }
}