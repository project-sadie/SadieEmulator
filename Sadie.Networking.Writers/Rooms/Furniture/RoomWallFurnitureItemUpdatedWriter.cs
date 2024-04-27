using Sadie.Database.Models.Rooms.Furniture;
using Sadie.Networking.Serialization;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Rooms.Furniture;

public class RoomWallFurnitureItemUpdatedWriter : AbstractPacketWriter
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