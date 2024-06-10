using Sadie.API.Game.Rooms.Chat.Commands;
using Sadie.API.Game.Rooms.Users;
using Sadie.Database;
using Sadie.Database.Models.Players;
using Sadie.Game.Players;
using Sadie.Game.Players.Packets;
using Sadie.Game.Rooms.Packets.Writers;
using Sadie.Shared.Unsorted;

namespace Sadie.Game.Rooms.Chat.Commands.Room;

public class PickAllCommand(PlayerRepository playerRepository, SadieContext dbContext) : AbstractRoomChatCommand, IRoomChatCommand
{
    public override string Trigger => "pickall";
    public override string Description => "Picks up all pieces of furniture in the room placing it into your inventory.";
    
    public override async Task ExecuteAsync(IRoomUser user, IEnumerable<string> parameters)
    {
        var player = user.Player;
        
        foreach (var item in user.Room.FurnitureItems.Where(x => x.FurnitureItem.Type == FurnitureItemType.Floor))
        {
            user.Room.FurnitureItems.Remove(item);
            dbContext.RoomFurnitureItems.Remove(item);

            var playerItem = new PlayerFurnitureItem
            {
                FurnitureItem = item.FurnitureItem,
                LimitedData = item.LimitedData,
                MetaData = item.MetaData,
                CreatedAt = DateTime.Now
            };
            
            var ownsItem = item.OwnerId == user.Id;
            
            if (ownsItem)
            {
                player.FurnitureItems.Add(playerItem);
            
                await player.NetworkObject.WriteToStreamAsync(new PlayerInventoryAddItemsWriter
                {
                    Items = [playerItem]
                });
                await player.NetworkObject.WriteToStreamAsync(new PlayerInventoryRefreshWriter());
            }
            else
            {
                var ownerOnline = playerRepository.GetPlayerLogicById(item.OwnerId);

                if (ownerOnline != null)
                {
                    var writer = new PlayerInventoryAddItemsWriter
                    {
                        Items = [playerItem]
                    };
                    
                    await ownerOnline.NetworkObject.WriteToStreamAsync(writer);
                    await ownerOnline.NetworkObject.WriteToStreamAsync(new PlayerInventoryRefreshWriter());
                
                    ownerOnline.FurnitureItems.Add(playerItem);
                }
                else
                {
                    dbContext.PlayerFurnitureItems.Add(playerItem);
                }
            }

            var itemRemovedWriter = new RoomFloorFurnitureItemRemovedWriter
            {
                Id = item.Id.ToString(),
                Expired = false,
                OwnerId = item.OwnerId,
                Delay = 0,
            };
            
            await user.Room.UserRepository.BroadcastDataAsync(itemRemovedWriter);
        }

        await dbContext.SaveChangesAsync();
    }

    public override List<string> PermissionsRequired { get; set; } = ["command_pickall"];
    public override bool BypassPermissionCheckIfRoomOwner => true;
}