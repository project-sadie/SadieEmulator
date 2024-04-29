using Sadie.Database;
using Sadie.Database.Models.Rooms.Rights;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Enums;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Rooms.Rights;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Rooms;
using Sadie.Networking.Writers.Rooms.Rights;

namespace Sadie.Networking.Events.Handlers.Rooms.Rights;

public class RoomRemoveUserRightsEventHandler(
    SadieContext dbContext,
    RoomRemoveUserRightsEventParser eventParser,
    RoomRepository roomRepository) : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.RoomRemoveUserRights;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);
        
        if (client.Player == null)
        {
            return;
        }
        
        var room = roomRepository.TryGetRoomById(client.Player.CurrentRoomId);

        if (room == null)
        {
            return;
        }
        
        foreach (var playerId in eventParser.Ids)
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
        if (room.UserRepository.TryGetById(playerId, out var roomUser))
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