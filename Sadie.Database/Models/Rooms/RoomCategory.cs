using System.ComponentModel.DataAnnotations.Schema;
using Sadie.Shared.Attributes;

namespace Sadie.Database.Models.Rooms;

public class RoomCategory
{
    [PacketData] public int Id { get; init; }
    [PacketData] public string? Caption { get; init; }
    [PacketData] public bool IsVisible { get; init; }
    [PacketData] [NotMapped] public bool Unknown1 { get; init; } = false;
    [PacketData] [NotMapped] public string? Unknown2 => Caption;
    [PacketData] [NotMapped] public string? Unknown3 => Caption!.StartsWith("${") ? "" : Caption;
    [PacketData] [NotMapped] public bool Unknown4 { get; init; } = false;
}