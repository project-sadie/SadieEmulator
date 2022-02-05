namespace Sadie.Game.Rooms;

public class RoomData : RoomSettings
{
    public long Id { get; }
    public string Name { get; }
    public RoomModel Model { get; }
    
    public RoomData(long id, string name, RoomModel model)
    {
        Id = id;
        Name = name;
        Model = model;
    }
}