namespace Sadie.Database.Models.Players;

public class Player
{
    public long Id { get; set; }
    public string Username { get; set; }
    public string? SsoToken { get; set; }
    public int RoleId { get; set; }
    public DateTime CreatedAt { get; set; }
    public PlayerData Data { get; set; }
    public PlayerAvatarData AvatarData { get; set; }
    public List<PlayerTag> Tags { get; set; }
    public List<PlayerRoomLike> RoomLikes { get; set; }
    public List<PlayerRelationship> Relationships { get; set; }
    public PlayerNavigatorSettings NavigatorSettings { get; set; }
    public PlayerGameSettings PlayerGameSettings { get; set; }
    public List<PlayerBadge> Badges { get; set; }
}