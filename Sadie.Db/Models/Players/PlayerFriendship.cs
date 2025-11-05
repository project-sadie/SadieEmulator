using System.ComponentModel;
using Sadie.Core.Enums.Game.Players;

namespace Sadie.Db.Models.Players;

public class PlayerFriendship
{
    public int Id { get; init; }
    public long OriginPlayerId { get; init; }
    public Player? OriginPlayer { get; init; }
    public long TargetPlayerId { get; init; }
    public Player? TargetPlayer { get; init; }
    [DefaultValue(1)] public PlayerFriendshipStatus Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}