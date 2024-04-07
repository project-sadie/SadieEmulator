namespace Sadie.Database.Models.Players;

public class Player
{
    public long Id { get; set; }
    public string Username { get; set; }
    public int RoleId { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<PlayerTag> Tags { get; set; }
}