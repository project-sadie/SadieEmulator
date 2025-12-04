using Sadie.API;
using Sadie.API.DTOs.Player.Furniture;
using Sadie.API.Interfaces.Networking;
using Sadie.Core.Enums.Game.Furniture;
using Sadie.Core.Shared.Attributes;
using Sadie.Core.Shared.Helpers;

namespace Sadie.Networking.Packets.Writers.Players.Inventory;

[PacketId(ServerPacketId.PlayerInventoryFurnitureItems)]
public class PlayerInventoryFurnitureItemsWriter : AbstractPacketWriter
{
    public required int Pages { get; init; }
    public required int CurrentPage { get; init; }
    public required List<PlayerFurnitureItemDto> Items { get; init; }

    public override void OnConfigureRules()
    {
        Override(GetType().GetProperty(nameof(Items))!, writer =>
        {
            writer.WriteInteger(Items.Count);

            foreach (var item in Items)
            {
                WriteItem(item, writer);
            }
        });
    }

    private static void WriteItem(PlayerFurnitureItemDto item, INetworkPacketWriter writer)
    {
        var furnitureItem = item.FurnitureItem;
        var hasRentPeriodStarted = false;
        var slotId = "";
        var extra = 1;
        var expiresInSeconds = -1;
        
        writer.WriteLong(item.Id);
        writer.WriteString(EnumHelpers.GetEnumDescription(furnitureItem.Type).ToUpper());
        writer.WriteLong(item.Id);
        writer.WriteInteger(furnitureItem.AssetId);
        
        switch (furnitureItem.AssetName)
        {
            case "floor":
                writer.WriteInteger(3);
                writer.WriteInteger(0);
                writer.WriteString(item.MetaData);
                break;
            case "wallpaper":
                writer.WriteInteger(2);
                writer. WriteInteger(0);
                writer. WriteString(item.MetaData);
                break;
            case "landscape":
                writer.WriteInteger(4);
                writer.WriteInteger(0);
                writer.WriteString(item.MetaData);
                break;
            default:
                writer.WriteInteger(1);
                writer.WriteInteger(1);
                writer.WriteInteger(0);
                break;
        }
        
        writer.WriteBool(furnitureItem.CanRecycle);
        writer.WriteBool(furnitureItem.CanTrade);
        writer.WriteBool(furnitureItem.CanInventoryStack);
        writer.WriteBool(furnitureItem.CanMarketplaceSell);
        writer.WriteInteger(expiresInSeconds);
        writer.WriteBool(hasRentPeriodStarted);
        writer.WriteInteger(-1);

        if (furnitureItem.Type != FurnitureItemType.Floor)
        {
            return;
        }
        
        writer.WriteString(slotId);
        writer.WriteInteger(extra);
    }
}