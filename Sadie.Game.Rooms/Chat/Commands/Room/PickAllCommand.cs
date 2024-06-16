using Microsoft.EntityFrameworkCore;
using Sadie.API.Game.Rooms.Chat.Commands;
using Sadie.API.Game.Rooms.Users;
using Sadie.Database;
using Sadie.Database.Models.Players.Furniture;
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
        var updateMap = new Dictionary<PlayerLogic, List<PlayerFurnitureItem>>();

        var floorItems = user.Room.FurnitureItems.Where(x => x.FurnitureItem.Type == FurnitureItemType.Floor).ToList();
        
        foreach (var item in floorItems)
        {
            var playerItem = item.PlayerFurnitureItem!;

            playerItem.PlacementData = null;
            user.Room.FurnitureItems.Remove(item);
            
            dbContext.Entry(item).State = EntityState.Deleted;
            
            var onlineOwner = playerRepository.GetPlayerLogicById(item.PlayerFurnitureItem.PlayerId);

            if (onlineOwner != null)
            {
                if (!updateMap.ContainsKey(onlineOwner))
                {
                    updateMap[onlineOwner] = [];
                }

                updateMap[onlineOwner].Add(item.PlayerFurnitureItem);
            }
            
            var itemRemovedWriter = new RoomFloorFurnitureItemRemovedWriter
            {
                Id = item.PlayerFurnitureItemId.ToString(),
                Expired = false,
                OwnerId = item.PlayerFurnitureItem.PlayerId,
                Delay = 0,
            };
            
            await user.Room.UserRepository.BroadcastDataAsync(itemRemovedWriter);
        }

        foreach (var (updatePlayer, updatedItems) in updateMap)
        {
            await updatePlayer.NetworkObject!.WriteToStreamAsync(new PlayerInventoryUnseenItemsWriter
            {
                Count = updatedItems.Count,
                Category = 1,
                FurnitureItems = updatedItems
            });
                
            await updatePlayer.NetworkObject!.WriteToStreamAsync(new PlayerInventoryRefreshWriter());
        }

        await dbContext.SaveChangesAsync();
    }

    public override List<string> PermissionsRequired { get; set; } = ["command_pickall"];
    public override bool BypassPermissionCheckIfRoomOwner => true;
}