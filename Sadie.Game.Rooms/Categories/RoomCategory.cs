namespace Sadie.Game.Rooms.Categories;

public class RoomCategory(int id, string caption, bool visible)
{
    public int Id { get; } = id;
    public string Caption { get; } = caption;
    public bool Visible { get; } = visible;
}