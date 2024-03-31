using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Parsers.Rooms.Furniture;

public class RoomWallItemUpdatedEventParser : INetworkPacketEventParser
{
    public int ItemId { get; private set; }
    public string WallPosition { get; private set; }

    public void Parse(INetworkPacketReader reader)
    {
        ItemId = reader.ReadInteger();
        WallPosition = reader.ReadString();
    }
}