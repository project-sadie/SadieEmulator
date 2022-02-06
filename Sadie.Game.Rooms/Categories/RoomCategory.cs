namespace Sadie.Game.Rooms.Categories;

public class RoomCategory
{
    public int Id { get; }
    public string Caption { get; }
    public bool Visible { get; }
    
    public RoomCategory(int id, string caption, bool visible)
    {
        Id = id;
        Caption = caption;
        Visible = visible;
    }
}