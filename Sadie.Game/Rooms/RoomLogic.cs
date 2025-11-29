using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Groups;
using Sadie.API.DTOs.Rooms;
using Sadie.API.Interfaces.Game.Rooms;
using Sadie.API.Interfaces.Game.Rooms.Bots;
using Sadie.API.Interfaces.Game.Rooms.Mapping;
using Sadie.API.Interfaces.Game.Rooms.Users;
using Sadie.API.Interfaces.Networking;
using Sadie.Db.Models.Rooms;
using Sadie.Game.Rooms.Mapping;
using Sadie.Networking.Serialization;
using Sadie.Networking.Writers.Rooms.Users;

namespace Sadie.Game.Rooms;

public class RoomLogic(
    RoomDto room,
    RoomTileMap tileMap,
    IRoomUserRepository userRepository,
    IRoomBotRepository botRepository,
    IEventLoopGroup eventLoopGroup)
    : Room, IRoomLogic
{
    public RoomDto Room { get; } = room;
    public IRoomTileMap TileMap { get; } = tileMap;
    public IRoomUserRepository UserRepository { get; } = userRepository;
    public IRoomBotRepository BotRepository { get; } = botRepository;
    public IChannelGroup ChannelGroup { get; set; } = new DefaultChannelGroup(eventLoopGroup.GetNext());

    public async ValueTask DisposeAsync()
    {
    }
    
    public async Task BroadcastDataAsync(AbstractPacketWriter writer, IReadOnlyCollection<long>? excludedIds = null)
    {
        var packet = await NetworkPacketWriterSerializer.SerializeAsync(writer);
        
        if (excludedIds == null || excludedIds.Count == 0)
        {
            ChannelGroup.WriteAndFlushAsync(packet);
            return;
        }
        
        var anyUser = UserRepository.GetAll().First();
        var executor = anyUser.NetworkObject.Channel.EventLoop;

        var filtered = new DefaultChannelGroup(executor);

        foreach (var user in UserRepository.GetAll())
        {
            if (!excludedIds.Contains(user.Player.Player.Id))
            {
                filtered.Add(user.NetworkObject.Channel);
            }
        }

        filtered.WriteAndFlushAsync(packet);
    }

    public async Task SendUserStatusUpdatesAsync()
    {
        await BroadcastDataAsync(
            new RoomUserStatusWriter
            {
                Users = UserRepository.GetAll()
            });
    }

    public async Task SendUserDataUpdatesAsync()
    {
        await BroadcastDataAsync(
            new RoomUserDataWriter
            {
                Users = UserRepository.GetAll()
            });
    }
}