using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sadie.API.Game.Rooms;
using Sadie.API.Networking.Client;
using Sadie.API.Networking.Events.Handlers;
using Sadie.Db;
using Sadie.Shared.Attributes;
using Sadie.Networking.Writers.Rooms.Users;

namespace Sadie.Networking.Events.Handlers.Rooms;

[PacketId(EventHandlerId.RoomForwardData)]
public class RoomForwardDataEventHandler(IRoomRepository roomRepository,
    IDbContextFactory<SadieContext> dbContextFactory,
    IMapper mapper) : INetworkPacketEventHandler
{
    public int RoomId { get; init; }
    public int EnterRoom { get; init; }
    public int ForwardRoom { get; init; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        var room = await RoomHelpers.TryLoadRoomByIdAsync(
            RoomId, 
            roomRepository, 
            dbContextFactory, 
            mapper);
        
        if (room == null)
        {
            return;
        }

        if (client.Player?.Data == null)
        {
            return;
        }

        var isOwner = room.OwnerId == client.Player.Id;
        
        await client.WriteToStreamAsync(new  RoomForwardDataWriter
        {
            Room = room,
            RoomForward = true,
            EnterRoom = EnterRoom != 0 || ForwardRoom != 1,
            IsOwner = isOwner
        });
    }
}