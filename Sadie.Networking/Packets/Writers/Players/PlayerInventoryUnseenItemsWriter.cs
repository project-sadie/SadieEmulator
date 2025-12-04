using Sadie.API;
using Sadie.API.DTOs.Player.Furniture;
using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Packets.Writers.Players;

[PacketId(ServerPacketId.PlayerInventoryUnseenItems)]
public class PlayerInventoryUnseenItemsWriter : AbstractPacketWriter
{
    public required int Count { get; init; }
    public required int Category { get; init; }
    public required List<PlayerFurnitureItemDto> FurnitureItems { get; init; }

    public override void OnSerialize(INetworkPacketWriter writer)
    {
        writer.WriteInteger(Count);

        for (var i = 0; i < Count; i++)
        {
            writer.WriteInteger(Category);
            writer.WriteInteger(FurnitureItems.Count);
            foreach (var item in FurnitureItems)
            {
                writer.WriteInteger(item.Id);
            }
        }
    }
}