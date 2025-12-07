using Sadie.API;
using Sadie.API.DTOs.Players.Furniture;
using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Writers.Rooms.Furniture;

[PacketId(ServerPacketId.RoomWallFurnitureItemRemoved)]
public class RoomWallFurnitureItemRemovedWriter : AbstractPacketWriter
{
    public required PlayerFurnitureItemPlacementDataDto Item { get; init; }

    public override void OnSerialize(INetworkPacketWriter writer)
    {
        writer.WriteString(Item.Id.ToString());
        writer.WriteLong(Item.PlayerFurnitureItem.PlayerId);
    }
}