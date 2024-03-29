using Sadie.Game.Rooms;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Parsers.Rooms;

public record RoomSettingsSaveParser
{
    public long RoomId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public RoomAccessType AccessType { get; set; }
    public string Password { get; set; }
    public int MaxUsers { get; set; }
    public int CategoryId { get; set; }
    public int TagsCount { get; set; }
    public int TradeOption { get; set; }
    public bool AllowPets { get; set; }
    public bool CanPetsEat { get; set; }
    public bool CanUsersOverlap { get; set; }
    public bool HideWall { get; set; }
    public int WallSize { get; set; }
    public int FloorSize { get; set; }
    public int WhoCanMute { get; set; }
    public int WhoCanKick { get; set; }
    public int WhoCanBan { get; set; }
    public int ChatType { get; set; }
    public int ChatWeight { get; set; }
    public int ChatSpeed { get; set; }
    public int ChatDistance { get; set; }
    public int ChatProtection { get; set; }

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
}