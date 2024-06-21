using Sadie.Database;
using Sadie.Database.Models.Rooms.Rights;
using Sadie.Enums.Game.Rooms;
using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Rooms;
using Sadie.Networking.Writers.Rooms.Rights;

namespace Sadie.Networking.Events.Handlers.Rooms.Rights;

[PacketId(EventHandlerId.RoomRemoveUserRights)]
public class RoomRemoveUserRightsEventHandler(
    SadieContext dbContext,
    RoomRepository roomRepository) : INetworkPacketEventHandler
{
    public required List<int> Ids { get; set; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        if (client.Player == null)
        {
            return;
        }
        
        var room = roomRepository.TryGetRoomById(client.Player.State.CurrentRoomId);

        if (room == null)
        {
            return;
        }
        
        foreach (var playerId in Ids)
        {
            var right = room.PlayerRights.FirstOrDefault(x => x.PlayerId == playerId);
            
            if (right == null)
            {
                continue;
            }
            
            await RemoveRoomPlayerRightAsync(playerId, room, right);
        }
    }

    private async Task RemoveRoomPlayerRightAsync(long playerId, RoomLogic room, RoomPlayerRight right)
    {
        if (room.UserRepository.TryGetById((int) playerId, out var roomUser))
        {
            roomUser!.ControllerLevel = RoomControllerLevel.None;
            roomUser.ApplyFlatCtrlStatus();
            
            await roomUser.NetworkObject.WriteToStreamAsync(new RoomRightsWriter
            {
                ControllerLevel = (int) roomUser.ControllerLevel
            });
        }
        
        room.PlayerRights.Remove(right);

        dbContext.RoomPlayerRights.Remove(right);
        await dbContext.SaveChangesAsync();

        await room.UserRepository.BroadcastDataAsync(
            new RoomRemoveUserRightsWriter
            {
                RoomId = room.Id,
                PlayerId = playerId
            });
    }
}