using Sadie.Db.Models.Players;

namespace Sadie.Db.Models;

public class Role
{
    public int Id { get; init; }
    public string? Name { get; init; }
    public ICollection<Player> Players { get; init; } = [];
    public ICollection<Permission> Permissions { get; init; } = [];
}