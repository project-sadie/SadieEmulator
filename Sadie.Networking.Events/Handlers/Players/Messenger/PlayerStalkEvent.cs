using Sadie.Game.Players;
using Sadie.Game.Players.Friendships;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players.Messenger;
using Sadie.Networking.Writers.Rooms;

namespace Sadie.Networking.Events.Handlers.Players.Messenger;

public class PlayerStalkEvent(IPlayerRepository playerRepository) : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client,
        INetworkPacketReader reader)
    {
        var playerId = reader.ReadInteger();

        var friend =
            client.Player.Data.FriendshipComponent.Friendships.FirstOrDefault(x => x.TargetData.Id == playerId);

        if (friend == null || friend.Status != PlayerFriendshipStatus.Accepted)
        {
            await client.WriteToStreamAsync(new PlayerStalkErrorWriter(PlayerStalkError.NotFriends).GetAllBytes());
            return;
        }

        if (!playerRepository.TryGetPlayerById(playerId, out var targetPlayer))
        {
            await client.WriteToStreamAsync(new PlayerStalkErrorWriter(PlayerStalkError.TargetOffline).GetAllBytes());
            return;
        }

        if (targetPlayer.Data.CurrentRoomId == 0)
        {
            await client.WriteToStreamAsync(new PlayerStalkErrorWriter(PlayerStalkError.TargetNotInRoom).GetAllBytes());
            return;
        }

        if (client.Player.Data.CurrentRoomId == targetPlayer.Data.CurrentRoomId)
        {
            return;
        }

        await client.WriteToStreamAsync(new RoomForwardEntryWriter(targetPlayer.Data.CurrentRoomId).GetAllBytes());
    }
}