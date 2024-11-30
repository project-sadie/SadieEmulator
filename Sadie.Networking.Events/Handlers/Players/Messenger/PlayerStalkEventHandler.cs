using Sadie.API.Game.Players;
using Sadie.Enums.Game.Players;
using Sadie.Game.Rooms.Packets.Writers;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Players.Messenger;

namespace Sadie.Networking.Events.Handlers.Players.Messenger;

[PacketId(EventHandlerId.PlayerStalk)]
public class PlayerStalkEventHandler(IPlayerRepository playerRepository) : INetworkPacketEventHandler
{
    public int PlayerId { get; set; }
    
    public async Task HandleAsync(INetworkClient client)
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

        if (targetPlayer.State.CurrentRoomId == 0)
        {
            await client.WriteToStreamAsync(new PlayerStalkErrorWriter
            {
                StalkError = (int) PlayerStalkError.TargetNotInRoom
            });
            
            return;
        }

        if (client.Player.State.CurrentRoomId == targetPlayer.State.CurrentRoomId)
        {
            return;
        }

        await client.WriteToStreamAsync(new RoomForwardEntryWriter
        {
            RoomId = targetPlayer.State.CurrentRoomId
        });
    }
}