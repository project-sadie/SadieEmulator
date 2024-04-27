using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;
using Sadie.Shared.Unsorted.Networking.Packets.Attributes;

namespace Sadie.Networking.Writers.Players.Other;

[PacketId(ServerPacketId.PlayerAchievementScore)]
public class PlayerAchievementScoreWriter : AbstractPacketWriter
{
    public required long AchievementScore { get; init; }
}