using Sadie.API.Game.Players;
using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Mapping;
using Sadie.Database;
using Sadie.Game.Rooms.Furniture;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Rooms.Doorbell;

namespace Sadie.Networking.Events.Handlers.Rooms.Doorbell;

[PacketId(EventHandlerId.RoomDoorbellAnswer)]
public class RoomDoorbellAnswerEventHandler(
    IPlayerRepository playerRepository,
    IRoomRepository roomRepository,
    SadieContext dbContext,
    RoomUserFactory roomUserFactory,
    INetworkClientRepository clientRepository,
    IRoomTileMapHelperService tileMapHelperService,
    IPlayerHelperService playerHelperService,
    IRoomFurnitureItemHelperService roomFurnitureItemHelperService) : INetworkPacketEventHandler
{
    public required string Username { get; set; }
    public bool Accept { get; set; }
    
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
                await RoomHelpersDirty.AfterEnterRoomAsync(
                    playerClient, 
                    room, 
                    roomUserFactory, 
                    dbContext, 
                    playerRepository,
                    tileMapHelperService,
                    playerHelperService,
                    roomFurnitureItemHelperService);
            }
            
            return;
        }

        await player.NetworkObject!.WriteToStreamAsync(new RoomDoorbellNoAnswerWriter { Username = Username });
    }
}