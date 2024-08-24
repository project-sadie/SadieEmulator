using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Players.Other;

[PacketId(ServerPacketId.PlayerAchievementScore)]
public class PlayerAchievementScoreWriter : AbstractPacketWriter
{
    public required long AchievementScore { get; init; }
}