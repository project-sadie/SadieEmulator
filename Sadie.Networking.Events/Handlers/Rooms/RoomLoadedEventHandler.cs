using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sadie.API.Game.Players;
using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Furniture;
using Sadie.API.Game.Rooms.Mapping;
using Sadie.API.Game.Rooms.Services;
using Sadie.API.Game.Rooms.Users;
using Sadie.Db;
using Sadie.Enums.Game.Rooms;
using Sadie.Enums.Miscellaneous;
using Sadie.Enums.Unsorted;
using Sadie.Networking.Client;
using Sadie.Shared.Attributes;
using Sadie.Networking.Writers.Generic;
using Sadie.Networking.Writers.Rooms;
using Sadie.Networking.Writers.Rooms.Doorbell;
using Sadie.Networking.Writers.Rooms.Users;

namespace Sadie.Networking.Events.Handlers.Rooms;

[PacketId(EventHandlerId.RoomLoaded)]
public class RoomLoadedEventHandler(
    ILogger<RoomLoadedEventHandler> logger,
    IRoomRepository roomRepository,
    IRoomUserFactory roomUserFactory,
    IPlayerRepository playerRepository,
    IDbContextFactory<SadieContext> dbContextFactory,
    IMapper mapper,
    IRoomTileMapHelperService tileMapHelperService,
    IPlayerHelperService playerHelperService,
    IRoomFurnitureItemHelperService roomFurnitureItemHelperService,
    IRoomWiredService wiredService)
    : INetworkPacketEventHandler
{
    public int RoomId { get; init; }
    public required string Password { get; init; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        var player = client.Player;

        if (player == null)
        {
            return;
        }

        var room = await RoomHelpers.TryLoadRoomByIdAsync(
            RoomId,
            roomRepository,
            dbContextFactory,
            mapper);
        
        var lastRoomId = player.State.CurrentRoomId;
        
        if (lastRoomId != 0)
        {
            var lastRoom = await RoomHelpers.TryLoadRoomByIdAsync(lastRoomId,
                roomRepository,
                dbContextFactory, 
                mapper);

            if (lastRoom != null && lastRoom.UserRepository.TryGetById(player.Id, out var existingUser) && existingUser != null)
            {
                await lastRoom.UserRepository.TryRemoveAsync(existingUser.Player.Id);
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
        
        await RoomEntryEventHelpers.GenericEnterRoomAsync(
            client, 
            room, 
            roomUserFactory, 
            dbContextFactory, 
            playerRepository,
            tileMapHelperService,
            playerHelperService,
            roomFurnitureItemHelperService,
            wiredService);
    }

    private static async Task<bool> ValidateRoomAccessForClientAsync(INetworkClient client, IRoomLogic room, string password)
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
                    ErrorCode = (int) GenericErrorCode.NavigatorInvalidPassword
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
            case RoomAccessType.Invisible:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return true;
    }
}