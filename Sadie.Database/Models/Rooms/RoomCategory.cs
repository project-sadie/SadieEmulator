using Sadie.Shared.Attributes;

namespace Sadie.Database.Models.Rooms;

public class RoomCategory
{
    [PacketData] public int Id { get; init; }
    [PacketData] public string? Caption { get; init; }
    [PacketData] public bool Visible { get; init; }
}