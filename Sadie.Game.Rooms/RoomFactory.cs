using Microsoft.Extensions.DependencyInjection;
using Sadie.Database.Models.Players;
using Sadie.Database.Models.Rooms;
using Sadie.Database.Models.Rooms.Chat;
using Sadie.Database.Models.Rooms.Furniture;
using Sadie.Database.Models.Rooms.Rights;
using Sadie.Shared.Unsorted.Game.Rooms;

namespace Sadie.Game.Rooms;

public class RoomFactory(IServiceProvider serviceProvider)
{
    public Room Create(int id,
        string name,
        RoomLayout layout,
        RoomLayoutData layoutData,
        int ownerId,
        string ownerUsername,
        string description,
        List<PlayerRoomLike> playerLikes,
        List<string> tags,
        int maxUsersAllowed,
        bool isMuted,
        RoomSettings settings,
        List<RoomChatMessage> chatMessages,
        List<RoomPlayerRight> playersWithRights,
        List<RoomFurnitureItem> furnitureItems,
        RoomPaintSettings paintSettings)
    {
        return ActivatorUtilities.CreateInstance<Room>(
            serviceProvider,
            id, 
            name,
            layout,
            layoutData,
            ownerId,
            ownerUsername,
            description,
            playerLikes,
            tags,
            maxUsersAllowed,
            isMuted,
            settings,
            chatMessages,
            playersWithRights,
            furnitureItems,
            paintSettings);
    }
}