using Sadie.Database;
using Sadie.Database.Models.Rooms.Rights;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Enums;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Rooms;
using Sadie.Networking.Writers.Rooms.Rights;

namespace Sadie.Networking.Events.Handlers.Rooms.Rights;

[PacketId(EventHandlerIds.RoomGiveUserRights)]
public class RoomGiveUserRightsEventHandler(
    SadieContext dbContext,
    RoomGiveUserRightsEventParser eventParser,
    RoomRepository roomRepository) : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);

        var playerId = eventParser.PlayerId;
        var player = client.Player;
        
        var room = roomRepository.TryGetRoomById(player.CurrentRoomId);

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