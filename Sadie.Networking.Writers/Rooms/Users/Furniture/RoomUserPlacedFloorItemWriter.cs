using Sadie.Game.Rooms.FurnitureItems;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms.Users.Furniture;

public class RoomUserPlacedFloorItemWriter : NetworkPacketWriter
{
    public RoomUserPlacedFloorItemWriter(RoomFurnitureItem roomFurnitureItem)
    {
        WriteShort(ServerPacketId.RoomUserPlacedFloorItem);
        WriteLong(roomFurnitureItem.Id);
        WriteInteger(roomFurnitureItem.FurnitureItem.AssetId);
        WriteInteger(roomFurnitureItem.Position.X);
        WriteInteger(roomFurnitureItem.Position.Y);
        WriteInteger((int) roomFurnitureItem.Direction);
        WriteString($"{roomFurnitureItem.Position.Z.ToString():0.00}");
        WriteString("");
        WriteInteger(1);
        WriteInteger(0);
        WriteString(roomFurnitureItem.MetaData);
        WriteInteger(-1);
        WriteInteger(1);
        WriteLong(roomFurnitureItem.OwnerId);
        WriteString(roomFurnitureItem.OwnerUsername);
    }
}