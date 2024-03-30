using Sadie.Game.Players.Badges;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Players;

public class PlayerWearingBadgesWriter : NetworkPacketWriter
{
    public PlayerWearingBadgesWriter(int playerId, List<PlayerBadge> badges)
    {
        WriteShort(ServerPacketId.PlayerBadges);
        WriteInteger(playerId);
        WriteInteger(badges.Count);

        foreach (var item in badges)
        {
            WriteInteger(item.Slot);
            WriteString(item.Code);
        }
    }
}