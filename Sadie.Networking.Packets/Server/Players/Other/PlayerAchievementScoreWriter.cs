namespace Sadie.Networking.Packets.Server.Players.Other;

internal class PlayerAchievementScoreWriter : NetworkPacketWriter
{
    internal PlayerAchievementScoreWriter(long achievementScore) : base(ServerPacketId.PlayerAchievementScore)
    {
        WriteLong(achievementScore);
    }
}