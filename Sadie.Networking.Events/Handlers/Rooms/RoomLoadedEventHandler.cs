using AutoMapper;
using Microsoft.Extensions.Logging;
using Sadie.API.Game.Rooms;
using Sadie.Database;
using Sadie.Enums.Game.Rooms;
using Sadie.Enums.Unsorted;
using Sadie.Game.Players;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Packets.Writers;
using Sadie.Game.Rooms.Packets.Writers.Users;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers;
using Sadie.Networking.Writers.Rooms;
using Sadie.Networking.Writers.Rooms.Doorbell;

namespace Sadie.Networking.Events.Handlers.Rooms;

[PacketId(EventHandlerId.RoomLoaded)]
public class RoomLoadedEventHandler(
    ILogger<RoomLoadedEventHandler> logger,
    RoomRepository roomRepository,
    RoomUserFactory roomUserFactory,
    PlayerRepository playerRepository,
    SadieContext dbContext,
    IMapper mapper)
    : INetworkPacketEventHandler
{
    public int RoomId { get; set; }
    public required string Password { get; set; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        var player = client.Player;

        if (player == null)
        {
            return;
        }

        var room = await Game.Rooms.RoomHelpersDirty.TryLoadRoomByIdAsync(
            RoomId,
            roomRepository,
            dbContext,
            mapper);
        
        var lastRoomId = player.State.CurrentRoomId;
        
        if (lastRoomId != 0)
        {
            var lastRoom = await Game.Rooms.RoomHelpersDirty.TryLoadRoomByIdAsync(lastRoomId,
                roomRepository,
                dbContext, 
                mapper);

            if (lastRoom != null && lastRoom.UserRepository.TryGetById(player.Id, out var oldUser) && oldUser != null)
            {
                await lastRoom.UserRepository.TryRemoveAsync(oldUser.Id, true);
            }
        }

        if (room == null)
        {
            logger.LogError($"Failed to load room {RoomId} for player '{player.Username}'");
            await client.WriteToStreamAsync(new RoomUserHotelViewWriter());
            
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
            !await ValidateRoomAccessForClientAsync(client, room, Password))
        {
            return;
        }
        
        await RoomHelpersDirty.AfterEnterRoomAsync(
            client, 
            room, 
            roomUserFactory, 
            dbContext, 
            playerRepository);
    }
    
    public static async Task<bool> ValidateRoomAccessForClientAsync(INetworkClient client, IRoomLogic room, string password)
    {
        var player = client.Player!;
        
        switch (room.Settings.AccessType)
        {
            case RoomAccessType.Password:
                if (room.Settings.Password == password)
                {
                    return true;
                }
                
                await client.WriteToStreamAsync(new GenericErrorWriter
                {
                    ErrorCode = (int) GenericErrorCode.IncorrectRoomPassword
                });
                
                await client.WriteToStreamAsync(new RoomUserHotelViewWriter());
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