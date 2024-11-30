using Microsoft.EntityFrameworkCore;
using Sadie.API;
using Sadie.API.Game.Players;
using Sadie.API.Game.Rooms.Users;
using Sadie.Database;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Game.Rooms.Packets.Writers;
using Sadie.Networking.Writers.Players;
using Sadie.Networking.Writers.Players.Inventory;

namespace CommandsPlugin.Room;

public class PickAllCommand(IPlayerRepository playerRepository, SadieContext dbContext) : AbstractRoomChatCommand
{
    public override string Trigger => "pickall";
    public override string Description => "Picks up all pieces of furniture in the room placing it into your inventory.";
    
    public override async Task ExecuteAsync(IRoomUser user, IEnumerable<string> parameters)
    {
        var updateMap = new Dictionary<IPlayerLogic, List<PlayerFurnitureItem>>();
        
        foreach (var item in user.Room.FurnitureItems.ToList())
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