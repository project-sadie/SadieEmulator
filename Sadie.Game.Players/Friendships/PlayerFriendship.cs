namespace Sadie.Game.Players.Friendships;

public class PlayerFriendship(
    int id,
    int originId,
    int targetId,
    PlayerFriendshipStatus status,
    PlayerFriendshipData targetData)
{
    public int Id { get; } = id;
    public PlayerFriendshipStatus Status { get; set; } = status;
    public int OriginId { get; } = originId;
    public int TargetId { get; } = targetId;
    public PlayerFriendshipData TargetData { get; } = targetData;
}