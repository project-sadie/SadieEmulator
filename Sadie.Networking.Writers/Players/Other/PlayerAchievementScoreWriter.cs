using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Writers.Players.Other;

[PacketId(ServerPacketId.PlayerAchievementScore)]
public class PlayerAchievementScoreWriter : AbstractPacketWriter
{
    public required long AchievementScore { get; init; }
}