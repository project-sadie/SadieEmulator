using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Parsers.Rooms.Furniture;

public class RoomFurnitureItemEjectedEventParser : INetworkPacketEventParser
{
    public int Category { get; private set; }
    public int ItemId { get; private set; }

    public void Parse(INetworkPacketReader reader)
    {
        Category = reader.ReadInt();
        ItemId = reader.ReadInt();
    }
}