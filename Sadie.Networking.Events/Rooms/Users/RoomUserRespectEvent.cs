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
        var playerData = player.Data;
        var targetId = reader.ReadInt();
        var lastRoom = player.Data.LastRoomLoaded;
        
        if (playerData.RespectPoints < 1 || 
            playerData.Id == targetId || 
            !_playerRepository.TryGetPlayerById(targetId, out var targetPlayer) || 
            targetPlayer!.Data.LastRoomLoaded != 0 && lastRoom != targetPlayer.Data.LastRoomLoaded)
        {
            return;
        }

        var targetData = targetPlayer.Data;

        playerData.RespectPoints--;
        targetData.RespectsReceived++;
        
        await _respectDao.CreateAsync(playerData.Id, targetId);

        await room!.UserRepository.BroadcastDataAsync(new RoomUserRespectWriter(targetId, targetData.RespectsReceived).GetAllBytes());
        await room!.UserRepository.BroadcastDataAsync(new RoomUserActionWriter(roomUser!.Id, RoomUserAction.ThumbsUp).GetAllBytes());
    }
}