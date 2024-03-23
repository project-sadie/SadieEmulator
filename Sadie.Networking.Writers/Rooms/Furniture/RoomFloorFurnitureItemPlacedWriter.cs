using Sadie.Game.Rooms.FurnitureItems;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms.Furniture;

public class RoomFloorFurnitureItemPlacedWriter : NetworkPacketWriter
{
    public RoomFloorFurnitureItemPlacedWriter(RoomFurnitureItem roomFurnitureItem)
    {
        WriteShort(ServerPacketId.RoomFloorFurnitureItemPlaced);
        WriteLong(roomFurnitureItem.Id);
        WriteInteger(roomFurnitureItem.FurnitureItem.AssetId);
        WriteInteger(roomFurnitureItem.Position.X);
        WriteInteger(roomFurnitureItem.Position.Y);
        WriteInteger((int) roomFurnitureItem.Direction);
        WriteString($"{roomFurnitureItem.Position.Z.ToString():0.00}");
        WriteString($"0"); // TODO: height
        WriteInteger(1);
        WriteInteger(0);
        WriteInteger(-1);
        WriteInteger(1);
        WriteLong(roomFurnitureItem.OwnerId);
        WriteString(roomFurnitureItem.OwnerUsername);
    }
}