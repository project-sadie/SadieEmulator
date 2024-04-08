using Sadie.Database.Models.Players;

namespace Sadie.Database.Models;

public class Role
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<Player> Players { get; set; }
    public List<Permission> Permissions { get; set; }
}