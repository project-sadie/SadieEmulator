using Sadie.Database.Models.Players;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Players;

public class PlayerWearingBadgesWriter : AbstractPacketWriter
{
    public PlayerWearingBadgesWriter(int playerId, ICollection<PlayerBadge> badges)
    {
        WriteShort(ServerPacketId.PlayerBadges);
        WriteInteger(playerId);
        WriteInteger(badges.Count);

        foreach (var item in badges)
        {
            WriteInteger(item.Slot);
            WriteString(item.Badge.Code);
        }
    }
}