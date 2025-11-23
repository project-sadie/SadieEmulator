using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sadie.API.DTOs.Rooms;
using Sadie.API.Interfaces.Game.Rooms;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Enums.Game.Rooms;
using Sadie.Core.Shared.Attributes;
using Sadie.Db;
using Sadie.Db.Models.Rooms;
using Sadie.Networking.Writers.Navigator;

namespace Sadie.Networking.Events.Handlers.Players;

[PacketId(EventHandlerId.PlayerCreateRoom)]
public class PlayerCreateRoomEventHandler(
    IDbContextFactory<SadieDbContext> dbContextFactory,
    IRoomRepository roomRepository,
    IMapper mapper) : INetworkPacketEventHandler
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string LayoutName { get; set; }
    public int CategoryId { get; set; }
    public int MaxUsersAllowed { get; set; }
    public int TradingPermission { get; set; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        
        var layout = dbContext
            .RoomLayouts
            .FirstOrDefault(x => x.Name == LayoutName);

        if (layout == null)
        {
            return;
        }

        var layoutDto = mapper.Map<RoomLayoutDto>(layout);
        
        var newRoom = new RoomDto
        {
            Name = Name,
            OwnerId = client.Player.Id,
            Layout = layoutDto,
            LayoutId = layout.Id,
            MaxUsersAllowed = MaxUsersAllowed,
            Description = Description,
            CreatedAt = DateTime.Now
        };

        newRoom.Settings = new RoomSettingsDto
        {
            RoomId = newRoom.Id,
            WalkDiagonal = true,
            TradeOption = RoomTradeOption.Allowed
        };

        newRoom.ChatSettings = new RoomChatSettingsDto
        {
            RoomId = newRoom.Id
        };

        newRoom.PaintSettings = new RoomPaintSettingsDto
        {
            RoomId = newRoom.Id
        };
        
        var newRoomEntity = mapper.Map<Room>(newRoom);
        dbContext.Rooms.Add(newRoomEntity);
        await dbContext.SaveChangesAsync();

        newRoom.OwnerId = client.Player.Id;
        newRoom.Layout = layoutDto;

        var roomLogic = mapper.Map<IRoomLogic>(newRoom);
            
        roomRepository.AddRoom(roomLogic);

        await client.WriteToStreamAsync(new RoomCreatedWriter
        {
            Id = newRoom.Id,
            Name = newRoom.Name
        });
    }
}