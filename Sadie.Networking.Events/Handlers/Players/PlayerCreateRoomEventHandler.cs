using AutoMapper;
using Sadie.API.Game.Rooms;
using Sadie.Database;
using Sadie.Database.Models.Players;
using Sadie.Database.Models.Rooms;
using Sadie.Enums.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Navigator;

namespace Sadie.Networking.Events.Handlers.Players;

[PacketId(EventHandlerId.PlayerCreateRoom)]
public class PlayerCreateRoomEventHandler(
    SadieContext dbContext,
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
        var layout = dbContext
            .RoomLayouts
            .FirstOrDefault(x => x.Name == LayoutName);

        if (layout == null)
        {
            return;
        }

        var newRoom = new Room
        {
            Name = Name,
            OwnerId = client.Player.Id,
            LayoutId = layout.Id,
            MaxUsersAllowed = 50,
            Description = Description,
            CreatedAt = DateTime.Now
        };

        newRoom.Settings = new RoomSettings
        {
            RoomId = newRoom.Id,
            WalkDiagonal = true,
            TradeOption = RoomTradeOption.Allowed
        };

        newRoom.ChatSettings = new RoomChatSettings
        {
            RoomId = newRoom.Id
        };

        newRoom.PaintSettings = new RoomPaintSettings
        {
            RoomId = newRoom.Id
        };
        
        dbContext.Rooms.Add(newRoom);
        await dbContext.SaveChangesAsync();

        newRoom.Owner = (Player) client.Player;
        newRoom.Layout = layout;

        var roomLogic = mapper.Map<IRoomLogic>(newRoom);
            
        roomRepository.AddRoom(roomLogic);
        client.Player.Rooms.Add(newRoom);

        await client.WriteToStreamAsync(new RoomCreatedWriter
        {
            Id = newRoom.Id,
            Name = newRoom.Name
        });
    }
}