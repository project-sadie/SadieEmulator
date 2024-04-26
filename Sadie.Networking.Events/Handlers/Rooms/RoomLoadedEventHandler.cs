using Microsoft.Extensions.Logging;
using Sadie.Game.Players;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Enums;
using Sadie.Game.Rooms.Packets.Writers;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Rooms;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Rooms;
using Sadie.Shared.Unsorted;

namespace Sadie.Networking.Events.Handlers.Rooms;

public class RoomLoadedEventHandler(
    RoomLoadedEventParser eventParser,
    ILogger<RoomLoadedEventHandler> logger,
    RoomRepository roomRepository,
    RoomUserFactory roomUserFactory,
    PlayerRepository playerRepository)
    : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.RoomLoaded;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader readers)
    {
        eventParser.Parse(readers);

        var player = client.Player;

        var (roomId, password) = (eventParser.RoomId, eventParser.Password);
        var room = await roomRepository.TryLoadRoomByIdAsync(roomId);
        var lastRoomId = player.CurrentRoomId;
        
        if (lastRoomId != 0)
        {
            var lastRoom = await roomRepository.TryLoadRoomByIdAsync(lastRoomId);

            if (lastRoom != null && lastRoom.UserRepository.TryGet(player.Id, out var oldUser) && oldUser != null)
            {
                await lastRoom.UserRepository.TryRemoveAsync(oldUser.Id);
            }
        }

        if (room == null)
        {
            logger.LogError($"Failed to load room {roomId} for player '{player.Username}'");
            await client.WriteToStreamAsync(new RoomUserHotelView());
            return;
        }

        var isOwner = room.OwnerId == player.Id;

        if (room.UserRepository.Count > room.MaxUsersAllowed && !isOwner)
        {
            await client.WriteToStreamAsync(new RoomEnterErrorWriter(RoomEnterError.NoCapacity));
            return;
        }

        if (room.Settings.AccessType is RoomAccessType.Doorbell or RoomAccessType.Password && 
            !isOwner && 
            !player.HasPermission("enter_guarded_rooms") && 
            !await NetworkPacketEventHelpers.ValidateRoomAccessForClientAsync(client, room, password))
        {
            return;
        }
        
        await NetworkPacketEventHelpers.EnterRoomAsync(client, room, logger, roomUserFactory);
        
        await playerRepository.UpdateMessengerStatusForFriends(
            player.Id,
            player.GetMergedFriendships(), 
            true, 
            true);
    }
}