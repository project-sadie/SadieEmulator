using Sadie.Database;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Packets.Writers;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted;

namespace Sadie.Networking.Events.Handlers.Rooms.Furniture;

[PacketId(EventHandlerIds.RoomWallItemUpdated)]
public class RoomWallItemUpdatedEventHandler(
    SadieContext dbContext,
    RoomRepository roomRepository)
    : INetworkPacketEventHandler
{
    public int ItemId { get; set; }
    public string WallPosition { get; set; }
    
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        if (client.Player == null || client.RoomUser == null)
        {
            return;
        }
        
        var room = roomRepository.TryGetRoomById(client.Player.CurrentRoomId);
        
        if (room == null || !client.RoomUser.HasRights())
        {
            return;
        }

        if (!client.RoomUser.HasRights())
        {
            await NetworkPacketEventHelpers.SendFurniturePlacementErrorAsync(client, FurniturePlacementError.MissingRights);
            return;
        }

        var roomFurnitureItem = room.FurnitureItems.FirstOrDefault(x => x.Id == ItemId);

        if (roomFurnitureItem == null)
        {
            return;
        }

        var wallPosition = WallPosition;

        if (string.IsNullOrEmpty(wallPosition))
        {
            return;
        }

        roomFurnitureItem.WallPosition = wallPosition;

        await dbContext.SaveChangesAsync();
        
        await room.UserRepository.BroadcastDataAsync(new RoomWallFurnitureItemUpdatedWriter
        {
            Item = roomFurnitureItem
        });
    }
}