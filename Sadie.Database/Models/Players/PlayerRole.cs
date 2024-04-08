namespace Sadie.Database.Models.Players;

public class PlayerRole
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<PlayerPermission> Permissions { get; set; }
}