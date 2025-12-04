using Sadie.API;
using Sadie.API.DTOs.Player.Furniture;
using Sadie.API.Interfaces.Game.Rooms.Furniture;
using Sadie.API.Interfaces.Networking;
using Sadie.Core.Enums.Miscellaneous;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Packets.Writers.Rooms.Furniture;

[PacketId(ServerPacketId.RoomFloorItems)]
public class RoomFloorItemsWriter : AbstractPacketWriter
{
    public required Dictionary<long, string?> FurnitureOwners { get; init; }
    public required ICollection<PlayerFurnitureItemPlacementDataDto> FloorItems { get; init; }
    public required IRoomFurnitureItemHelperService RoomFurnitureItemHelperService { get; init; }

    public override void OnSerialize(INetworkPacketWriter writer)
    {
        writer.WriteInteger(FurnitureOwners.Count);

        foreach (var owner in FurnitureOwners)
        {
           writer.WriteLong(owner.Key);
           writer.WriteString(owner.Value ?? "");
        }
        
        writer.WriteInteger(FloorItems.Count);

        foreach (var item in FloorItems)
        {
            WriteItem(item, writer);
        }
    }

    private void WriteItem(PlayerFurnitureItemPlacementDataDto item, INetworkPacketWriter writer)
    {
        var height = -1; // TODO: height
        var extra = 1;
            
        writer.WriteLong(item.PlayerFurnitureItem.Id);
        writer.WriteInteger(item.PlayerFurnitureItem.FurnitureItem.AssetId);
        writer.WriteInteger(item.PositionX);
        writer.WriteInteger(item.PositionY);
        writer.WriteInteger((int) item.Direction);
        writer.WriteString($"{item.PositionZ.ToString():0.00}");
        writer.WriteString(height.ToString());
        writer.WriteInteger(extra);
        
        var objectDataKey = RoomFurnitureItemHelperService.GetObjectDataKeyForItem(item);
        
        
        writer.WriteInteger((int) objectDataKey);

        if (objectDataKey == ObjectDataKey.MapKey)
        {
            var objectData = RoomFurnitureItemHelperService.GetObjectDataForItem(item);
            
            writer.WriteInteger(objectData.Count);

            foreach (var pair in objectData)
            {
                writer.WriteString(pair.Key);
                writer.WriteString(pair.Value);
            }
        }
        else
        {
            writer.WriteString(item.PlayerFurnitureItem.MetaData);
        }
        
        writer.WriteInteger(-1);
        writer.WriteInteger(item.PlayerFurnitureItem.FurnitureItem.InteractionModes > 1 ? 1 : 0); 
        writer.WriteLong(item.PlayerFurnitureItem.PlayerId);
    }
}