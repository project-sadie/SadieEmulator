using Microsoft.Extensions.DependencyInjection;
using Sadie.Database.Models;
using Sadie.Database.Models.Players;
using Sadie.Database.Models.Rooms;
using Sadie.Database.Models.Rooms.Chat;
using Sadie.Database.Models.Rooms.Furniture;
using Sadie.Database.Models.Rooms.Rights;
using Sadie.Game.Rooms.Users;
using Sadie.Shared.Unsorted.Game.Rooms;

namespace Sadie.Game.Rooms;

public class RoomFactory(IServiceProvider serviceProvider)
{
    public RoomLogic Create(int id,
        string name,
        RoomLayout layout,
        RoomLayoutData layoutData,
        Player owner,
        string description,
        int maxUsersAllowed,
        bool isMuted,
        List<RoomFurnitureItem> furnitureItems,
        RoomSettings settings,
        List<RoomChatMessage> chatMessages,
        List<RoomPlayerRight> playerRights,
        RoomPaintSettings paintSettings,
        List<RoomTag> tags,
        List<PlayerRoomLike> playerLikes)
    {
        return ActivatorUtilities.CreateInstance<RoomLogic>(
            serviceProvider,
            id, 
            name,
            layout,
            layoutData,
            owner,
            description,
            playerLikes,
            tags,
            maxUsersAllowed,
            isMuted,
            settings,
            chatMessages,
            playerRights,
            furnitureItems,
            paintSettings);
    }
}