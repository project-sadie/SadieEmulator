using Sadie.Database;
using Sadie.Database.Models.Rooms.Rights;
using Sadie.Enums.Game.Rooms;
using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Rooms;
using Sadie.Networking.Writers.Rooms.Rights;

namespace Sadie.Networking.Events.Handlers.Rooms.Rights;

[PacketId(EventHandlerIds.RoomGiveUserRights)]
public class RoomGiveUserRightsEventHandler(
    SadieContext dbContext,
    RoomRepository roomRepository) : INetworkPacketEventHandler
{
    public int PlayerId { get; set; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        var playerId = PlayerId;
        var player = client.Player;
        
        var room = roomRepository.TryGetRoomById(player.State.CurrentRoomId);

        if (room == null)
        {
            return;
        }

        if (room.PlayerRights.FirstOrDefault(x => x.PlayerId == playerId) != null)
        {
            return;
        }

        await room.UserRepository.BroadcastDataAsync(new RoomGiveUserRightsWriter 
        {
            RoomId = room.Id,
            PlayerId = playerId,
            PlayerUsername = player.Username
        });

        if (room.UserRepository.TryGetById(playerId, out var targetRoomUser))
        {
            targetRoomUser!.ControllerLevel = RoomControllerLevel.Rights;
            targetRoomUser.ApplyFlatCtrlStatus();
            
            await targetRoomUser.NetworkObject.WriteToStreamAsync(new RoomRightsWriter
            {
                ControllerLevel = (int) targetRoomUser.ControllerLevel
            });
        }
        
        var roomPlayerRight = new RoomPlayerRight
        {
            RoomId = room.Id,
            PlayerId = playerId
        };
        
        room.PlayerRights.Add(roomPlayerRight);
        
        dbContext.RoomPlayerRights.Add(roomPlayerRight);
        await dbContext.SaveChangesAsync();
    }
}