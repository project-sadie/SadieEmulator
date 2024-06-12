using Sadie.Game.Players;
using Sadie.Game.Rooms.Packets.Writers;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Players.Messenger;
using Sadie.Networking.Writers.Rooms;

namespace Sadie.Networking.Events.Handlers.Players.Messenger;

[PacketId(EventHandlerIds.PlayerStalk)]
public class PlayerStalkEventHandler(PlayerRepository playerRepository) : INetworkPacketEventHandler
{
    public int PlayerId { get; set; }
    
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var playerId = PlayerId;

        if (!client.Player.IsFriendsWith(PlayerId))
        {
            await client.WriteToStreamAsync(new PlayerStalkErrorWriter
            {
                StalkError = (int) PlayerStalkError.NotFriends
            });
            
            return;
        }

        var targetPlayer = playerRepository.GetPlayerLogicById(playerId);
        
        if (targetPlayer == null)
        {
            await client.WriteToStreamAsync(new PlayerStalkErrorWriter
            {
                StalkError = (int) PlayerStalkError.TargetOffline
            });
            
            return;
        }

        if (targetPlayer.CurrentRoomId == 0)
        {
            await client.WriteToStreamAsync(new PlayerStalkErrorWriter
            {
                StalkError = (int) PlayerStalkError.TargetNotInRoom
            });
            
            return;
        }

        if (client.Player.CurrentRoomId == targetPlayer.CurrentRoomId)
        {
            return;
        }

        await client.WriteToStreamAsync(new RoomForwardEntryWriter
        {
            RoomId = targetPlayer.CurrentRoomId
        });
    }
}