namespace Sadie.Database.Models.Rooms.Furniture;

public class RoomFurnitureItemTeleportLink
{
    public required int Id { get; init; }
    public required int ParentId { get; init; }
    public required RoomFurnitureItem Parent { get; init; }
    public required int ChildId { get; init; }
    public required RoomFurnitureItem Child { get; init; }
}