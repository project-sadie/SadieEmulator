using Sadie.API.Game.Rooms;
using Sadie.Database;
using Sadie.Game.Players;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Rooms.Doorbell;

namespace Sadie.Networking.Events.Handlers.Rooms.Doorbell;

[PacketId(EventHandlerId.RoomDoorbellAnswer)]
public class RoomDoorbellAnswerEventHandler(
    PlayerRepository playerRepository,
    IRoomRepository roomRepository,
    SadieContext dbContext,
    RoomUserFactory roomUserFactory,
    INetworkClientRepository clientRepository) : INetworkPacketEventHandler
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
                    playerRepository);
            }
            
            return;
        }

        await player.NetworkObject!.WriteToStreamAsync(new RoomDoorbellNoAnswerWriter { Username = Username });
    }
}