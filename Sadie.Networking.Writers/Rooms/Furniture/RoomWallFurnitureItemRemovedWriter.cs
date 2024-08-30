using Sadie.API;
using Sadie.API.Networking;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Rooms.Furniture;

[PacketId(ServerPacketId.RoomWallFurnitureItemRemoved)]
public class RoomWallFurnitureItemRemovedWriter : AbstractPacketWriter
{
    public required PlayerFurnitureItemPlacementData Item { get; init; }

    public override void OnSerialize(INetworkPacketWriter writer)
    {
        writer.WriteString(Item.Id.ToString());
        writer.WriteLong(Item.PlayerFurnitureItem.PlayerId);
    }
}