using Microsoft.EntityFrameworkCore;
using Sadie.API.Game.Rooms.Chat.Commands;
using Sadie.API.Game.Rooms.Users;
using Sadie.Database;
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
            var playerItem = item.PlayerFurnitureItem;
            var ownsItem = item.PlayerFurnitureItem.PlayerId == user.Id;

            playerItem.PlacementData.Remove(item);
            user.Room.FurnitureItems.Remove(item);
            
            dbContext.Entry(item).State = EntityState.Deleted;
            
            if (ownsItem)
            {
                await player.NetworkObject.WriteToStreamAsync(new PlayerInventoryAddItemsWriter
                {
                    FurnitureItems = [playerItem]
                });
                
                await player.NetworkObject.WriteToStreamAsync(new PlayerInventoryRefreshWriter());
            }
            else
            {
                var ownerOnline = playerRepository.GetPlayerLogicById(item.PlayerFurnitureItem.PlayerId);

                if (ownerOnline != null)
                {
                    var writer = new PlayerInventoryAddItemsWriter
                    {
                        FurnitureItems = [playerItem]
                    };
                    
                    await ownerOnline.NetworkObject.WriteToStreamAsync(writer);
                    await ownerOnline.NetworkObject.WriteToStreamAsync(new PlayerInventoryRefreshWriter());
                }
            }

            var itemRemovedWriter = new RoomFloorFurnitureItemRemovedWriter
            {
                Id = item.Id.ToString(),
                Expired = false,
                OwnerId = item.PlayerFurnitureItem.PlayerId,
                Delay = 0,
            };
            
            await user.Room.UserRepository.BroadcastDataAsync(itemRemovedWriter);
        }

        await dbContext.SaveChangesAsync();
    }

    public override List<string> PermissionsRequired { get; set; } = ["command_pickall"];
    public override bool BypassPermissionCheckIfRoomOwner => true;
}