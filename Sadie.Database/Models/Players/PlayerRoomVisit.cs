using System.ComponentModel.DataAnnotations;

namespace Sadie.Database.Models.Players;

public class PlayerRoomVisit
{
    [Key] public int Id { get; init; }
    public required long PlayerId { get; init; }
    public required int RoomId { get; init; }
    public required DateTime CreatedAt { get; init; }
}