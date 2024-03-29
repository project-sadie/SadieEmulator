using Sadie.Networking.Packets;
using Sadie.Shared.Unsorted.Game.Rooms;

namespace Sadie.Networking.Events.Parsers.Rooms.Furniture;

public class RoomFloorFurnitureItemUpdatedParser
{
    public int ItemId { get; private set; }
    public int X { get; private set; }
    public int Y { get; private set; }
    public HDirection Direction { get; private set; }

    public void Parse(INetworkPacketReader reader)
    {
        ItemId = reader.ReadInteger();
        X = reader.ReadInteger();
        Y = reader.ReadInteger();
        Direction = (HDirection) reader.ReadInteger();
    }
}