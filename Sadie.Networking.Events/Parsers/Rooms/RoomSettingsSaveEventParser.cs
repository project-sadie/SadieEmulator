using Sadie.Networking.Packets;
using Sadie.Shared.Unsorted;

namespace Sadie.Networking.Events.Parsers.Rooms;

public record RoomSettingsSaveEventParser : INetworkPacketEventParser
{
    public void Parse(INetworkPacketReader reader)
    {
        RoomId = reader.ReadInt();
        Name = reader.ReadString();
        Description = reader.ReadString();
        AccessType = (RoomAccessType) reader.ReadInt();
        Password = reader.ReadString();
        MaxUsers = reader.ReadInt();
        CategoryId = reader.ReadInt();
        
        var tagCount = reader.ReadInt();

        for (var i = 0; i < tagCount; i++)
        {
            var tag = reader.ReadString();

            if (string.IsNullOrEmpty(tag))
            {
                continue;
            }
            
            Tags.Add(tag);
        }
        
        TradeOption = reader.ReadInt();
        AllowPets = reader.ReadBool();
        CanPetsEat = reader.ReadBool();
        CanUsersOverlap = reader.ReadBool();
        HideWall = reader.ReadBool();
        WallSize = reader.ReadInt();
        FloorSize = reader.ReadInt();
        WhoCanMute = reader.ReadInt();
        WhoCanKick = reader.ReadInt();
        WhoCanBan = reader.ReadInt();
        ChatType = reader.ReadInt();
        ChatWeight = reader.ReadInt();
        ChatSpeed = reader.ReadInt();
        ChatDistance = reader.ReadInt();
        ChatProtection = reader.ReadInt();
    }
    
    public long RoomId { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public RoomAccessType AccessType { get; private set; }
    public string Password { get; private set; }
    public int MaxUsers { get; private set; }
    public int CategoryId { get; private set; }
    public List<string> Tags { get; private set; } = [];
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