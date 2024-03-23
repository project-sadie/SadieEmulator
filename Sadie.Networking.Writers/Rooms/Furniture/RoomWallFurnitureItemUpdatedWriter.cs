using Sadie.Game.Rooms.FurnitureItems;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms.Furniture;

public class RoomWallFurnitureItemUpdatedWriter : NetworkPacketWriter
{
    public RoomWallFurnitureItemUpdatedWriter(RoomFurnitureItem item)
    {
        WriteShort(ServerPacketId.RoomWallFurnitureItemUpdated);
        WriteString(item.Id + "");
        WriteInteger(item.FurnitureItem.AssetId);
        WriteString(item.WallPosition);
        WriteString(item.MetaData);
        WriteInteger(-1);
        WriteInteger(item.FurnitureItem.InteractionModes > 1 ? 1 : 0);
        WriteLong(item.OwnerId);
        WriteString(item.OwnerUsername);
    }
}