using Microsoft.Extensions.Logging;
using Sadie.Shared;
using Sadie.Shared.Networking;

namespace Sadie.Game.Rooms.Users;

public class RoomUser : RoomUserData, IDisposable
{
    private readonly ILogger<RoomUser> _logger;
    private readonly IRoomUserRepository _roomUserRepository;
    public INetworkObject NetworkObject { get; }

    public RoomUser(
        ILogger<RoomUser> logger,
        IRoomUserRepository roomUserRepository,
        INetworkObject networkObject, 
        long id, 
        HPoint point, 
        HDirection directionHead, 
        HDirection direction, 
        string username, 
        string motto, 
        string figureCode, 
        string gender, 
        long achievementScore) : 
        base(id, point, directionHead, direction, username, motto, figureCode, gender, achievementScore)
    {
        _logger = logger;
        _roomUserRepository = roomUserRepository;
        NetworkObject = networkObject;
    }

    public void Dispose()
    {
        if (!_roomUserRepository.TryRemove(Id))
        {
            _logger.LogError($"Failed to dispose room user {Id}");
        }
    }
}