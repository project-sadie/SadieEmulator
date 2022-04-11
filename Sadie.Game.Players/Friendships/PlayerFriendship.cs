namespace Sadie.Game.Players.Friendships;

public class PlayerFriendship
{
    public int Id { get; }
    public PlayerFriendshipStatus Status { get; set; }
    public PlayerFriendshipType Type { get; }
    public int OriginId { get; }
    public int TargetId { get; }
    public PlayerFriendshipData TargetData { get; }

    public PlayerFriendship(int id, int originId, int targetId, PlayerFriendshipStatus status, PlayerFriendshipType type, PlayerFriendshipData targetData)
    {
        Id = id;       
        Status = status;
        Type = type;
        OriginId = originId;
        TargetId = targetId;
        TargetData = targetData;
    }
}