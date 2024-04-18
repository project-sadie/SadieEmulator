using Sadie.Database.Models.Players;

namespace Sadie.Database.Models;

public class Role
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<Player> Players { get; set; } = [];
    public ICollection<Permission> Permissions { get; set; } = [];
}