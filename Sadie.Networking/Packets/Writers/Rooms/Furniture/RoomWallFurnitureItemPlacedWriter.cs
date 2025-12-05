using Sadie.API;
using Sadie.API.DTOs.Players.Furniture;
using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Packets.Writers.Rooms.Furniture;

[PacketId(ServerPacketId.RoomWallFurnitureItemPlaced)]
public class RoomWallFurnitureItemPlacedWriter : AbstractPacketWriter
{
    public required PlayerFurnitureItemPlacementDataDto RoomFurnitureItem { get; init; }
    public required string OwnerUsername { get; init; }

    public override void OnSerialize(INetworkPacketWriter writer)
    {
        var furnitureItem = RoomFurnitureItem.PlayerFurnitureItem.FurnitureItem;
        
        writer.WriteString(RoomFurnitureItem.Id + "");
        writer.WriteInteger(furnitureItem.AssetId);
        writer.WriteString(RoomFurnitureItem.WallPosition ?? "");
        writer.WriteString(RoomFurnitureItem.PlayerFurnitureItem.MetaData);
        writer.WriteInteger(-1);
        writer.WriteInteger(furnitureItem.InteractionModes > 1 ? 1 : 0);
        writer.WriteLong(RoomFurnitureItem.Id);
        writer.WriteString(OwnerUsername);
    }
}