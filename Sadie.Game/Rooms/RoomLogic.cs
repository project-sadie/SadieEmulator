using Sadie.API.DTOs.Rooms;
using Sadie.API.Interfaces.Game.Rooms;
using Sadie.API.Interfaces.Game.Rooms.Bots;
using Sadie.API.Interfaces.Game.Rooms.Mapping;
using Sadie.API.Interfaces.Game.Rooms.Pathfinding;
using Sadie.API.Interfaces.Game.Rooms.Users;
using Sadie.API.Interfaces.Networking;
using Sadie.Db.Models.Rooms;
using Sadie.Networking.Packets.Serialization;

namespace Sadie.Game.Rooms;

public class RoomLogic(
    RoomDto room,
    IRoomTileMap tileMap,
    IRoomPathFinder pathFinder,
    IRoomUserRepository userRepository,
    IRoomBotRepository botRepository)
    : Room, IRoomLogic
{
    public RoomDto Room { get; } = room;
    public IRoomTileMap TileMap { get; } = tileMap;
    public IRoomPathFinder PathFinder { get; } = pathFinder;
    public IRoomUserRepository UserRepository { get; } = userRepository;
    public IRoomBotRepository BotRepository { get; } = botRepository;
    
    public async ValueTask DisposeAsync()
    {
    }
    
    public async Task BroadcastDataAsync(AbstractPacketWriter writer, IReadOnlyCollection<long>? excludedIds = null)
    {
        var packet = NetworkPacketWriterSerializer.Serialize(writer);
        
        var usersToBroadcastTo = excludedIds == null || excludedIds.Count == 0
            ? UserRepository.GetAll()
            : UserRepository.GetAll().Where(x => !excludedIds.Contains(x.Player.Player.Id));
        
        foreach (var user in usersToBroadcastTo)
        {
            await user.NetworkObject.WriteToStreamAsync(packet);
        }
    }
}