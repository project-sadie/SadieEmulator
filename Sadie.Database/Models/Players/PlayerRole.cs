namespace Sadie.Database.Models.Players;

public class PlayerRole
{
    public int Id { get; }
    public string Name { get; }
    public List<PlayerPermission> Permissions { get; set; }
}