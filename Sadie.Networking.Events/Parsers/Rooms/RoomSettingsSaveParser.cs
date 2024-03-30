using Sadie.Game.Rooms;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Parsers.Rooms;

public record RoomSettingsSaveParser
{
    public void Parse(INetworkPacketReader reader)
    {
        RoomId = reader.ReadInteger();
        Name = reader.ReadString();
        Description = reader.ReadString();
        AccessType = (RoomAccessType) reader.ReadInteger();
        Password = reader.ReadString();
        MaxUsers = reader.ReadInteger();
        CategoryId = reader.ReadInteger();
        TagsCount = reader.ReadInteger();
        TradeOption = reader.ReadInteger();
        AllowPets = reader.ReadBool();
        CanPetsEat = reader.ReadBool();
        CanUsersOverlap = reader.ReadBool();
        HideWall = reader.ReadBool();
        WallSize = reader.ReadInteger();
        FloorSize = reader.ReadInteger();
        WhoCanMute = reader.ReadInteger();
        WhoCanKick = reader.ReadInteger();
        WhoCanBan = reader.ReadInteger();
        ChatType = reader.ReadInteger();
        ChatWeight = reader.ReadInteger();
        ChatSpeed = reader.ReadInteger();
        ChatDistance = reader.ReadInteger();
        ChatProtection = reader.ReadInteger();
    }
    
    public long RoomId { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public RoomAccessType AccessType { get; private set; }
    public string Password { get; private set; }
    public int MaxUsers { get; private set; }
    public int CategoryId { get; private set; }
    public int TagsCount { get; private set; }
    public int TradeOption { get; private set; }
    public bool AllowPets { get; private set; }
    public bool CanPetsEat { get; private set; }
    public bool CanUsersOverlap { get; private set; }
    public bool HideWall { get; private set; }
    public int WallSize { get; private set; }
    public int FloorSize { get; private set; }
    public int WhoCanMute { get; private set; }
    public int WhoCanKick { get; private set; }
    public int WhoCanBan { get; private set; }
    public int ChatType { get; private set; }
    public int ChatWeight { get; private set; }
    public int ChatSpeed { get; private set; }
    public int ChatDistance { get; private set; }
    public int ChatProtection { get; private set; }

}