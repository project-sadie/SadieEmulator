using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Parsers.Rooms.Furniture;

public class RoomFurnitureItemPlacedEventParser : INetworkPacketEventParser
{
    public string[] PlacementData { get; private set; }

    public void Parse(INetworkPacketReader reader)
    {
        PlacementData = reader.ReadString().Split(" ");
    }
}