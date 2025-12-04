using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sadie.API.Interfaces.Game.Players;
using Sadie.API.Interfaces.Game.Rooms;
using Sadie.API.Interfaces.Game.Rooms.Furniture;
using Sadie.API.Interfaces.Game.Rooms.Mapping;
using Sadie.API.Interfaces.Game.Rooms.Services;
using Sadie.API.Interfaces.Game.Rooms.Users;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Shared.Attributes;
using Sadie.Db;
using Sadie.Networking.Packets.Writers.Rooms.Doorbell;

namespace Sadie.Networking.Events.Handlers.Rooms.Doorbell;

[PacketId(EventHandlerId.RoomDoorbellAnswer)]
public class RoomDoorbellAnswerEventHandler(
    IPlayerRepository playerRepository,
    IRoomRepository roomRepository,
    IDbContextFactory<SadieDbContext> dbContextFactory,
    IRoomUserFactory roomUserFactory,
    INetworkClientRepository clientRepository,
    IRoomTileMapHelperService tileMapHelperService,
    IPlayerHelperService playerHelperService,
    IRoomFurnitureItemHelperService roomFurnitureItemHelperService,
    IRoomWiredService wiredService,
    IMapper mapper) : INetworkPacketEventHandler
{
    public required string Username { get; init; }
    public bool Accept { get; init; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        if (!NetworkPacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out var room, out _))
        {
            return;
        }
        
        var player = playerRepository.GetPlayerLogicByUsername(Username);
        
        if (player == null)
        {
            return;
        }

        if (Accept)
        {
            await player.NetworkObject!.WriteToStreamAsync(new RoomDoorbellAcceptWriter
            {
                Username = Username
            });

            var playerClient = clientRepository.TryGetClientByGuid(player.NetworkObject.Guid);

            if (playerClient != null)
            {
                await RoomEntryEventHelpers.GenericEnterRoomAsync(
                    playerClient, 
                    room, 
                    roomUserFactory, 
                    dbContextFactory, 
                    playerRepository,
                    tileMapHelperService,
                    playerHelperService,
                    roomFurnitureItemHelperService,
                    wiredService,
                    mapper);
            }
            
            return;
        }

        await player.NetworkObject!.WriteToStreamAsync(new RoomDoorbellNoAnswerWriter { Username = Username });
    }
}