using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Parsers.Players;

public class PlayerCreateRoomEventParser : INetworkPacketEventParser
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string LayoutName { get; private set; }
    public int CategoryId { get; private set; }
    public int MaxUsersAllowed { get; private set; }
    public int TradingPermission { get; private set; }
    public int LayoutId { get; private set; }

    public void Parse(INetworkPacketReader reader)
    {
        Name = reader.ReadString();
        Description = reader.ReadString();
        LayoutName = reader.ReadString();
        CategoryId = reader.ReadInteger();
        MaxUsersAllowed = reader.ReadInteger();
        TradingPermission = reader.ReadInteger();
        LayoutId = reader.ReadInteger();
    }
}