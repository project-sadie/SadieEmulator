using Sadie.API;
using Sadie.API.DTOs.Players.Furniture;
using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Packets.Writers.Rooms.Furniture;

[PacketId(ServerPacketId.RoomWallFurnitureItemUpdated)]
public class RoomWallFurnitureItemUpdatedWriter : AbstractPacketWriter
{
    public required PlayerFurnitureItemPlacementDataDto Item { get; init; }
    public required string OwnerUsername { get; init; }

    public override void OnSerialize(INetworkPacketWriter writer)
    {
        var furnitureItem = Item.PlayerFurnitureItem.FurnitureItem;
        
        writer.WriteString(Item.Id + "");
        writer.WriteInteger(furnitureItem.AssetId);
        writer.WriteString(Item.WallPosition ?? "");
        writer.WriteString(Item.PlayerFurnitureItem.MetaData);
        writer.WriteInteger(-1);
        writer.WriteInteger(furnitureItem.InteractionModes > 1 ? 1 : 0);
        writer.WriteLong(Item.PlayerFurnitureItem.PlayerId);
        writer.WriteString(OwnerUsername);
    }
}