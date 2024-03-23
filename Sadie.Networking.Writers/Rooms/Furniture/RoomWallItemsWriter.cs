using Sadie.Game.Rooms.FurnitureItems;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms.Furniture;

public class RoomWallItemsWriter : NetworkPacketWriter
{
    public RoomWallItemsWriter(ICollection<RoomFurnitureItem> wallItems,
        Dictionary<long, string> furnitureOwners)
    {
       WriteShort(ServerPacketId.RoomWallItems);
       WriteInteger(furnitureOwners.Count);

       foreach (var owner in furnitureOwners)
       {
           WriteLong(owner.Key);
           WriteString(owner.Value);
       }
       
       WriteInteger(wallItems.Count);

       foreach (var item in wallItems)
       {
           WriteString(item.Id + "");
           WriteInteger(item.FurnitureItem.AssetId);
           WriteString(item.WallPosition);
           WriteString(item.MetaData);
           WriteInteger(-1);
           WriteInteger(item.FurnitureItem.InteractionModes > 1 ? 1 : 0);
           WriteLong(item.OwnerId);
       }
    }
}