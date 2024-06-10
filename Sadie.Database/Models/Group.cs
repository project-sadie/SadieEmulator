using Sadie.Database.Models.Players;

namespace Sadie.Database.Models;

public class Group
{
    public int Id { get; set; }
    public int PlayerId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int RoomId { get; set; }
    public int CreatedAt { get; set; }
    public ICollection<Player> Players { get; init; } = [];
}