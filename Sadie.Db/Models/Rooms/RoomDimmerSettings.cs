using System.ComponentModel.DataAnnotations;

namespace Sadie.Db.Models.Rooms;

public class RoomDimmerSettings
{
    [Key] public int Id { get; init; }
    public required int RoomId { get; init; }
    public required bool Enabled { get; set; }
    public required int PresetId { get; init; }
}