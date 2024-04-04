using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Parsers.Rooms.Furniture;

public class RoomPaintItemPlacedEventParser : INetworkPacketEventParser
{
    public int ItemId { get; private set; }

    public void Parse(INetworkPacketReader reader)
    {
        ItemId = reader.ReadInteger();
    }
}