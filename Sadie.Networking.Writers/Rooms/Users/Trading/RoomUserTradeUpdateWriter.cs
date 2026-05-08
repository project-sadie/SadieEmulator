using Sadie.API;
using Sadie.API.Interfaces.Game.Rooms.Users;
using Sadie.API.Interfaces.Networking;
using Sadie.Core.Enums.Game.Furniture;
using Sadie.Core.Shared.Attributes;
using Sadie.Core.Shared.Helpers;

namespace Sadie.Networking.Writers.Rooms.Users.Trading;

[PacketId(ServerPacketId.RoomUserTradeUpdate)]
public class RoomUserTradeUpdateWriter : AbstractPacketWriter
{
    public required IRoomUserTrade Trade { get; init; }

    public override void OnSerialize(INetworkPacketWriter writer)
    {
        foreach (var user in Trade.Users)
        {
            writer.WriteLong(user.Player.Player.Id);

            var usersOfferedItems = Trade
                .Items
                .Where(x => x.PlayerId == user.Player.Player.Id)
                .ToList();
            
            writer.WriteInteger(usersOfferedItems.Count);
            
            foreach (var item in usersOfferedItems)
            {
                writer.WriteString(EnumHelpers.GetEnumDescription(item.FurnitureItem.Type));
                writer.WriteInteger(item.Id);
                writer.WriteInteger(item.Id);
                writer.WriteInteger(item.FurnitureItem.AssetId);
                writer.WriteInteger(0);
                writer.WriteBool(item.FurnitureItem.CanInventoryStack);
                writer.WriteInteger(0);
                writer.WriteString(item.MetaData);
                writer.WriteInteger(0);
                writer.WriteInteger(0);
                writer.WriteInteger(0);

                if (item.FurnitureItem.Type == FurnitureItemType.Floor)
                {
                    writer.WriteInteger(0);
                }
            }
            
            writer.WriteInteger(usersOfferedItems.Count);
            writer.WriteInteger(0);
        }
    }
}