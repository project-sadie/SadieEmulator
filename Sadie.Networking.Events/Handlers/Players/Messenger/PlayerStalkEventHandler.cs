using Sadie.Game.Players;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Players.Messenger;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players.Messenger;
using Sadie.Networking.Writers.Rooms;
using Sadie.Shared.Unsorted;

namespace Sadie.Networking.Events.Handlers.Players.Messenger;

public class PlayerStalkEventHandler(PlayerStalkEventParser eventParser, PlayerRepository playerRepository) : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.PlayerStalk;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);

        var playerId = eventParser.PlayerId;

        if (!client.Player.IsFriendsWith(eventParser.PlayerId))
        {
            await client.WriteToStreamAsync(new PlayerStalkErrorWriter(PlayerStalkError.NotFriends).GetAllBytes());
            return;
        }

        if (!playerRepository.TryGetPlayerById(playerId, out var targetPlayer))
        {
            await client.WriteToStreamAsync(new PlayerStalkErrorWriter(PlayerStalkError.TargetOffline).GetAllBytes());
            return;
        }

        if (targetPlayer.CurrentRoomId == 0)
        {
            await client.WriteToStreamAsync(new PlayerStalkErrorWriter(PlayerStalkError.TargetNotInRoom).GetAllBytes());
            return;
        }

        if (client.Player.CurrentRoomId == targetPlayer.CurrentRoomId)
        {
            return;
        }

        await client.WriteToStreamAsync(new RoomForwardEntryWriter(targetPlayer.CurrentRoomId).GetAllBytes());
    }
}