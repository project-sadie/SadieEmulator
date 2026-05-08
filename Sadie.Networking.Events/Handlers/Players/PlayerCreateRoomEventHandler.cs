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
        if (client.Player == null)
        {
            return;
        }
        
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        var layout = await dbContext.RoomLayouts
            .FirstOrDefaultAsync(x => x.Name == LayoutName);

        if (layout == null)
        {
            return;
        }

        var roomEntity = new Room
        {
            Name = Name,
            OwnerId = client.Player.Player.Id,
            Description = Description,
            LayoutId = layout.Id,
            CreatedAt = DateTime.UtcNow,
            MaxUsersAllowed = MaxUsersAllowed,

            Settings = new RoomSettings
            {
                WalkDiagonal = true,
                TradeOption = RoomTradeOption.Allowed
            },

            ChatSettings = new RoomChatSettings(),
            PaintSettings = new RoomPaintSettings()
        };

        dbContext.Rooms.Add(roomEntity);
        await dbContext.SaveChangesAsync();

        var roomDto = mapper.Map<RoomDto>(roomEntity);
        roomDto.Layout = mapper.Map<RoomLayoutDto>(layout);

        var roomLogic = mapper.Map<IRoomLogic>(roomDto);
        roomLogic.UserRepository.SetRoom(roomLogic);
        roomRepository.AddRoom(roomLogic);

        await client.WriteToStreamAsync(new RoomCreatedWriter
        {
            Id = roomDto.Id,
            Name = roomDto.Name
        });
    }
}
