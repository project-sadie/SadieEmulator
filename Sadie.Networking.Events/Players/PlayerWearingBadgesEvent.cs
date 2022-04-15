using Sadie.Game.Players;
using Sadie.Game.Players.Badges;
using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players;

namespace Sadie.Networking.Events.Players;

public class PlayerWearingBadgesEvent : INetworkPacketEvent
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IPlayerBadgeRepository _badgeRepository;

    public PlayerWearingBadgesEvent(IPlayerRepository playerRepository, IRoomRepository roomRepository, IPlayerBadgeRepository badgeRepository)
    {
        _playerRepository = playerRepository;
        _roomRepository = roomRepository;
        _badgeRepository = badgeRepository;
    }
    
    public async Task HandleAsync(INetworkClient networkClient, INetworkPacketReader reader)
    {
        var playerId = reader.ReadInteger();
        var isPlayerOnline = _playerRepository.TryGetPlayerById(playerId, out var player);
        
        var playerBadges = isPlayerOnline ? 
            player!.Data.Badges : 
            await _badgeRepository.GetBadgesForPlayerAsync(playerId);

        playerBadges = playerBadges.
            Where(x => x.Slot != 0 && x.Slot <= 5).
            DistinctBy(x => x.Slot).
            ToList();
        
        if (!PacketEventHelpers.TryResolveRoomObjectsForClient(_roomRepository, networkClient, out var room, out _))
        {
            await networkClient.WriteToStreamAsync(new PlayerWearingBadgesWriter(playerId, playerBadges).GetAllBytes());
            return;
        }
        
        await networkClient.WriteToStreamAsync(new PlayerWearingBadgesWriter(playerId, playerBadges).GetAllBytes());
    }
}