using Sadie.Database.Models.Players.Furniture;
using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Game.Rooms.Packets.Writers;

[PacketId(ServerPacketId.RoomWallFurnitureItemUpdated)]
public class RoomWallFurnitureItemUpdatedWriter : AbstractPacketWriter
{
    public required PlayerFurnitureItemPlacementData Item { get; init; }

    public override void OnSerialize(NetworkPacketWriter writer)
    {
        writer.WriteString(Item.Id + "");
        writer.WriteInteger(Item.FurnitureItem.AssetId);
        writer.WriteString(Item.WallPosition);
        writer.WriteString(Item.PlayerFurnitureItem.MetaData);
        writer.WriteInteger(-1);
        writer.WriteInteger(Item.FurnitureItem.InteractionModes > 1 ? 1 : 0);
        writer.WriteLong(Item.PlayerFurnitureItem.Player.Id);
        writer.WriteString(Item.PlayerFurnitureItem.Player.Username);
    }
}