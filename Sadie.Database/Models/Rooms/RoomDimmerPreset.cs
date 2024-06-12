using System.ComponentModel.DataAnnotations;

namespace Sadie.Database.Models.Rooms;

public class RoomDimmerPreset
{
    [Key] public int Id { get; init; }
    public required long RoomId { get; init; }
    public required int PresetId { get; init; }
    public required bool BackgroundOnly { get; init; }
    public required string Color { get; init; }
    public required int Intensity { get; init; }
}