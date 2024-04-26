using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Other;

public class PlayerAchievementScoreWriter : AbstractPacketWriter
{
    public PlayerAchievementScoreWriter(long achievementScore)
    {
        WriteShort(ServerPacketId.PlayerAchievementScore);
        WriteLong(achievementScore);
    }
}