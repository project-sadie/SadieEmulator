using Microsoft.Extensions.Logging;
using Sadie.Database;
using Sadie.Game.Players;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Enums;
using Sadie.Game.Rooms.Packets.Writers;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers;
using Sadie.Networking.Writers.Rooms;
using Sadie.Networking.Writers.Rooms.Doorbell;
using Sadie.Shared.Unsorted;

namespace Sadie.Networking.Events.Handlers.Rooms;

[PacketId(EventHandlerIds.RoomLoaded)]
public class RoomLoadedEventHandler(
    ILogger<RoomLoadedEventHandler> logger,
    RoomRepository roomRepository,
    RoomUserFactory roomUserFactory,
    PlayerRepository playerRepository,
    SadieContext dbContext)
    : INetworkPacketEventHandler
{
    public int RoomId { get; set; }
    public required string Password { get; set; }
    
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader readers)
    {
        var player = client.Player;

        var (roomId, password) = (RoomId, Password);
        var room = await roomRepository.TryLoadRoomByIdAsync(roomId);
        var lastRoomId = player.CurrentRoomId;
        
        if (lastRoomId != 0)
        {
            var lastRoom = await roomRepository.TryLoadRoomByIdAsync(lastRoomId);

            if (lastRoom != null && lastRoom.UserRepository.TryGetById(player.Id, out var oldUser) && oldUser != null)
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
            await client.WriteToStreamAsync(new RoomEnterErrorWriter
            {
                ErrorCode = (int) RoomEnterError.NoCapacity
            });
            
            return;
        }

        if (room.Settings.AccessType is RoomAccessType.Doorbell or RoomAccessType.Password && 
            !isOwner && 
            !player.HasPermission("enter_guarded_rooms") && 
            !await ValidateRoomAccessForClientAsync(client, room, password))
        {
            return;
        }
        
        await RoomHelpersDirty.EnterRoomAsync(client, room, logger, roomUserFactory, dbContext);

        var friends = player
            .GetMergedFriendships()
            .Where(x => x.Status == PlayerFriendshipStatus.Accepted)
            .ToList();
        
        await playerRepository.UpdateStatusForFriendsAsync(
            player,
            friends, 
            true, 
            true);
    }
    
    public static async Task<bool> ValidateRoomAccessForClientAsync(INetworkClient client, RoomLogic room, string password)
    {
        var player = client.Player!;
        
        switch (room.Settings.AccessType)
        {
            case RoomAccessType.Password when password != room.Settings.Password:
                await client.WriteToStreamAsync(new GenericErrorWriter
                {
                    ErrorCode = (int) GenericErrorCode.IncorrectRoomPassword
                });
                await client.WriteToStreamAsync(new RoomUserHotelView());
                return false;
            
            case RoomAccessType.Doorbell:
            {
                var usersWithRights = room.UserRepository.GetAllWithRights();

                if (usersWithRights.Count < 1)
                {
                    await client.WriteToStreamAsync(new RoomDoorbellNoAnswerWriter
                    {
                        Username = player.Username
                    });
                    
                    return false;
                }
                
                foreach (var user in usersWithRights)
                {
                    await user.NetworkObject.WriteToStreamAsync(new RoomDoorbellWriter
                    {
                        Username = player.Username
                    });
                    
                }

                await client.WriteToStreamAsync(new RoomDoorbellWriter
                {
                    Username = ""
                });
                
                return false;
            }
            case RoomAccessType.Open:
                break;
            case RoomAccessType.Invisible:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return true;
    }
}