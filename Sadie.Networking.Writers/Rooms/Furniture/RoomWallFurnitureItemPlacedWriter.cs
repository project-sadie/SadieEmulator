using Sadie.API;
using Sadie.API.Networking;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Rooms.Furniture;

[PacketId(ServerPacketId.RoomWallFurnitureItemPlaced)]
public class RoomWallFurnitureItemPlacedWriter : AbstractPacketWriter
{
    public required PlayerFurnitureItemPlacementData RoomFurnitureItem { get; init; }

    public override void OnSerialize(INetworkPacketWriter writer)
    {
        writer.WriteString(RoomFurnitureItem.Id + "");
        writer.WriteInteger(RoomFurnitureItem.FurnitureItem.AssetId);
        writer.WriteString(RoomFurnitureItem.WallPosition);
        writer.WriteString(RoomFurnitureItem.PlayerFurnitureItem.MetaData);
        writer.WriteInteger(-1);
        writer.WriteInteger(RoomFurnitureItem.FurnitureItem.InteractionModes > 1 ? 1 : 0);
        writer.WriteLong(RoomFurnitureItem.Id);
        writer.WriteString(RoomFurnitureItem.PlayerFurnitureItem.Player.Username);
    }
}