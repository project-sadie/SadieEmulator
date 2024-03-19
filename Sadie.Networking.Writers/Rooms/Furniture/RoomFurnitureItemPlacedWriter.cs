using Sadie.Game.Rooms.FurnitureItems;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms.Furniture;

public class RoomFurnitureItemPlacedWriter : NetworkPacketWriter
{
    public RoomFurnitureItemPlacedWriter(RoomFurnitureItem roomFurnitureItem)
    {
        WriteShort(ServerPacketId.RoomFurnitureItemPlaced);
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