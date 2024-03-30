using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Other;

public class PlayerAchievementScoreWriter : NetworkPacketWriter
{
    public PlayerAchievementScoreWriter(long achievementScore)
    {
        WriteShort(ServerPacketId.PlayerAchievementScore);
        WriteLong(achievementScore);
    }
}