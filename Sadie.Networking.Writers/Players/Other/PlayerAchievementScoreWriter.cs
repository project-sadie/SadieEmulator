namespace Sadie.Networking.Packets.Server.Players.Other;

public class PlayerAchievementScoreWriter : NetworkPacketWriter
{
    public PlayerAchievementScoreWriter(long achievementScore) : base(ServerPacketId.PlayerAchievementScore)
    {
        WriteLong(achievementScore);
    }
}