using Sadie.Game.Rooms.FurnitureItems;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms.Furniture;

public class RoomWallFurnitureItemPlacedWriter : NetworkPacketWriter
{
    public RoomWallFurnitureItemPlacedWriter(RoomFurnitureItem roomFurnitureItem)
    {
        WriteShort(ServerPacketId.RoomWallFurnitureItemPlaced);
        WriteString(roomFurnitureItem.Id + "");
        WriteInteger(roomFurnitureItem.FurnitureItem.AssetId);
        WriteString(roomFurnitureItem.WallPosition);
        WriteString(roomFurnitureItem.MetaData);
        WriteInteger(-1);
        WriteInteger(roomFurnitureItem.FurnitureItem.InteractionModes > 1 ? 1 : 0);
        WriteLong(roomFurnitureItem.OwnerId);
        WriteString(roomFurnitureItem.OwnerUsername);
    }
}