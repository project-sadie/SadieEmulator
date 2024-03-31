using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Parsers.Rooms.Furniture;

public class RoomFurnitureItemUseEventParser : INetworkPacketEventParser
{
    public int ItemId { get; private set; }
    public int State { get; private set; }

    public void Parse(INetworkPacketReader reader)
    {
        ItemId = reader.ReadInteger();
        State = reader.ReadInteger();
    }
}