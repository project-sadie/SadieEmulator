using Sadie.Database.Models.Players;

namespace Sadie.Database.Models;

public class Role
{
    public int Id { get; init; }
    public ICollection<Player> Players { get; init; } = [];
    public ICollection<Permission> Permissions { get; init; } = [];
}