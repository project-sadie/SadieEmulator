using System.ComponentModel.DataAnnotations;

namespace Sadie.Database.Models.Rooms;

public class RoomDimmerPreset
{
    [Key] public int Id { get; init; }
    public required long RoomId { get; init; }
    public required int PresetId { get; init; }
    public required bool BackgroundOnly { get; set; }
    public required string Color { get; set; }
    public required int Intensity { get; set; }
}