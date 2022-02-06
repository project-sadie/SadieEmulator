namespace Sadie.Game.Rooms;

public class RoomData : RoomSettings
{
    public long Id { get; }
    public string Name { get; }
    public RoomLayout Layout { get; }

    protected RoomData(long id, string name, RoomLayout layout)
    {
        Id = id;
        Name = name;
        Layout = layout;
    }
}