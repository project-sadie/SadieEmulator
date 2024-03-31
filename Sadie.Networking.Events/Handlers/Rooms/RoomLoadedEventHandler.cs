using Microsoft.Extensions.Logging;
using Sadie.Game.Players;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Packets.Writers;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Rooms;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers;
using Sadie.Networking.Writers.Rooms;
using Sadie.Networking.Writers.Rooms.Doorbell;
using Sadie.Shared.Unsorted;

namespace Sadie.Networking.Events.Handlers.Rooms;

public class RoomLoadedEventHandler(
    RoomLoadedEventParser eventParser,
    ILogger<RoomLoadedEventHandler> logger,
    IRoomRepository roomRepository,
    IRoomUserFactory roomUserFactory,
    IPlayerRepository playerRepository)
    : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader readers)
    {
        eventParser.Parse(readers);

        var player = client.Player;
        var playerData = player.Data;

        var (roomId, password) = (eventParser.RoomId, eventParser.Password);
        var (found, room) = await roomRepository.TryLoadRoomByIdAsync(roomId);
        var lastRoomId = player.Data.CurrentRoomId;
        
        if (lastRoomId != 0)
        {
            var (foundLast, lastRoom) = await roomRepository.TryLoadRoomByIdAsync(lastRoomId);

            if (foundLast && lastRoom != null && lastRoom.UserRepository.TryGet(playerData.Id, out var oldUser) && oldUser != null)
            {
                var currentTile = lastRoom.Layout.FindTile(oldUser.Point.X, oldUser.Point.Y);
                currentTile?.Users.Remove(oldUser.Id);
                RoomHelpers.UpdateTileMapForTile(currentTile!, lastRoom.Layout);
                
                await lastRoom.UserRepository.TryRemoveAsync(oldUser.Id);
            }
        }

        if (!found || room == null)
        {
            logger.LogError($"Failed to load room {roomId} for player '{playerData.Username}'");
            await client.WriteToStreamAsync(new PlayerHotelViewWriter().GetAllBytes());
            return;
        }

        var isOwner = room.OwnerId == playerData.Id;

        if (room.UserRepository.Count > room.MaxUsers && !isOwner)
        {
            await client.WriteToStreamAsync(new RoomEnterErrorWriter(RoomEnterError.NoCapacity).GetAllBytes());
            return;
        }

        if (room.Settings.AccessType is RoomAccessType.Doorbell or RoomAccessType.Password && 
            !isOwner &&
            !playerData.Permissions.Contains("enter_guarded_rooms"))
        {
            switch (room.Settings.AccessType)
            {
                case RoomAccessType.Password when password != room.Settings.Password:
                    await client.WriteToStreamAsync(new GenericErrorWriter(GenericErrorCode.IncorrectRoomPassword).GetAllBytes());
                    await client.WriteToStreamAsync(new PlayerHotelViewWriter().GetAllBytes());
                    return;
                case RoomAccessType.Doorbell:
                {
                    var usersWithRights = room.UserRepository.GetAllWithRights();
                
                    if (usersWithRights.Count < 1)
                    {
                        await client.WriteToStreamAsync(new RoomDoorbellNoAnswerWriter(player.Data.Username).GetAllBytes());
                    }
                    else
                    {
                        foreach (var user in usersWithRights)
                        {
                            await user.NetworkObject.WriteToStreamAsync(new RoomDoorbellWriter(playerData.Username)
                                .GetAllBytes());
                        }

                        await client.WriteToStreamAsync(new RoomDoorbellWriter().GetAllBytes());
                    }

                    return;
                }
            }
        }
        
        await PacketEventHelpers.EnterRoomAsync(client, room, logger, roomUserFactory);
        
        await playerRepository.UpdateMessengerStatusForFriends(playerData.Id,
            playerData.FriendshipComponent.Friendships, true, true);
    }
}