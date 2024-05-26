using Sadie.Database;
using Sadie.Database.Models.Rooms;
using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Navigator;

namespace Sadie.Networking.Events.Handlers.Players;

[PacketId(EventHandlerIds.PlayerCreateRoom)]
public class PlayerCreateRoomEventHandler(
    SadieContext dbContext,
    RoomRepository roomRepository) : INetworkPacketEventHandler
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string LayoutName { get; set; }
    public int CategoryId { get; set; }
    public int MaxUsersAllowed { get; set; }
    public int TradingPermission { get; set; }
    
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
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
            RoomId = newRoom.Id
        };

        newRoom.ChatSettings = new RoomChatSettings
        {
            RoomId = newRoom.Id
        };

        newRoom.PaintSettings = new RoomPaintSettings
        {
            RoomId = newRoom.Id,
        };
        
        dbContext.Rooms.Add(newRoom);
        await dbContext.SaveChangesAsync();

        newRoom.Owner = client.Player;
        newRoom.Layout = layout;
        
        roomRepository.AddRoom(newRoom);
        client.Player.Rooms.Add(newRoom);

        await client.WriteToStreamAsync(new RoomCreatedWriter
        {
            Id = newRoom.Id,
            Name = newRoom.Name
        });
    }
}