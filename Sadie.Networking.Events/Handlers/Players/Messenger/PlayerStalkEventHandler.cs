using Sadie.Game.Players;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Players.Messenger;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Players.Messenger;
using Sadie.Networking.Writers.Rooms;

namespace Sadie.Networking.Events.Handlers.Players.Messenger;

[PacketId(EventHandlerIds.PlayerStalk)]
public class PlayerStalkEventHandler(PlayerStalkEventParser eventParser, PlayerRepository playerRepository) : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);

        var playerId = eventParser.PlayerId;

        if (!client.Player.IsFriendsWith(eventParser.PlayerId))
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