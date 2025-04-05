using System.ComponentModel.DataAnnotations;

namespace Sadie.Database.Models.Players;

public class PlayerRoomBan
{
    [Key] public int Id { get; init; }
    public long PlayerId { get; init; }
    public Player? Player { get; init; }
    public int RoomId { get; init; }
    public DateTime ExpiresAt { get; init; }
}