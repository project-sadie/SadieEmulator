using System.ComponentModel;

namespace Sadie.Db.Models.Rooms;

public class RoomLayout
{
    public int Id { get; init; }
    public string? Name { get; init; }
    public string? Heightmap { get; set; }
    public int DoorX { get; set; }
    public int DoorY { get; set; }
    public int DoorDirection { get; set; }
    [DefaultValue(false)] public bool RequiresClubMembership { get; set; }
    public string? ExtraData { get; set; }
}