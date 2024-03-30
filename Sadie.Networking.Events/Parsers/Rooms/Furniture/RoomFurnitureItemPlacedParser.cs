using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Parsers.Rooms.Furniture;

public class RoomFurnitureItemPlacedParser
{
    public string[] PlacementData { get; private set; }

    public void Parse(INetworkPacketReader reader)
    {
        PlacementData = reader.ReadString().Split(" ");
    }
}