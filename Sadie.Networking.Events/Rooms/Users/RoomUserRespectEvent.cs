using Sadie.Game.Players;
using Sadie.Game.Players.Respect;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Rooms.Users;

namespace Sadie.Networking.Events.Rooms.Users;

public class RoomUserRespectEvent : INetworkPacketEvent
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IPlayerRespectDao _respectDao;

    public RoomUserRespectEvent(IPlayerRepository playerRepository, IRoomRepository roomRepository, IPlayerRespectDao respectDao)
    {
        _playerRepository = playerRepository;
        _roomRepository = roomRepository;
        _respectDao = respectDao;
    }
    
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        if (!PacketEventHelpers.TryResolveRoomObjectsForClient(_roomRepository, client, out var room, out var roomUser))
        {
            return;
        }
        
        var player = client.Player!;
        var targetId = reader.ReadInt();
        
        if (player.RespectPoints < 1 || 
            client.Player!.Id == targetId || 
            !_playerRepository.TryGetPlayerById(targetId, out var targetPlayer) || 
            targetPlayer!.LastRoomLoaded != 0 && player.LastRoomLoaded != targetPlayer.LastRoomLoaded)
        {
            return;
        }

        player.RespectPoints--;
        targetPlayer.RespectsReceived++;
        
        await _respectDao.CreateAsync(player.Id, targetId);

        await room!.UserRepository.BroadcastDataAsync(new RoomUserRespectWriter(targetId, targetPlayer.RespectsReceived).GetAllBytes());
        await room!.UserRepository.BroadcastDataAsync(new RoomUserActionWriter(roomUser!.Id, RoomUserAction.ThumbsUp).GetAllBytes());
    }
}