using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Other;

public class PlayerAchievementScoreWriter : NetworkPacketWriter
{
    public PlayerAchievementScoreWriter(long achievementScore) : base(ServerPacketId.PlayerAchievementScore)
    {
        WriteLong(achievementScore);
    }
}