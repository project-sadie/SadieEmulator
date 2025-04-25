using Microsoft.EntityFrameworkCore;
using Sadie.API.Game.Players;
using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Furniture;
using Sadie.API.Game.Rooms.Mapping;
using Sadie.API.Game.Rooms.Services;
using Sadie.API.Game.Rooms.Users;
using Sadie.Database;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Rooms.Doorbell;

namespace Sadie.Networking.Events.Handlers.Rooms.Doorbell;

[PacketId(EventHandlerId.RoomDoorbellAnswer)]
public class RoomDoorbellAnswerEventHandler(
    IPlayerRepository playerRepository,
    IRoomRepository roomRepository,
    IDbContextFactory<SadieContext> dbContextFactory,
    IRoomUserFactory roomUserFactory,
    INetworkClientRepository clientRepository,
    IRoomTileMapHelperService tileMapHelperService,
    IPlayerHelperService playerHelperService,
    IRoomFurnitureItemHelperService roomFurnitureItemHelperService,
    IRoomWiredService wiredService) : INetworkPacketEventHandler
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

            var playerClient = clientRepository.TryGetClientByChannelId(player.Channel!.Id);

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
                    wiredService);
            }
            
            return;
        }

        await player.NetworkObject!.WriteToStreamAsync(new RoomDoorbellNoAnswerWriter { Username = Username });
    }
}