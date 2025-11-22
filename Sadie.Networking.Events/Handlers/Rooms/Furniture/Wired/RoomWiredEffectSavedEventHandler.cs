using Microsoft.EntityFrameworkCore;
using Sadie.API.Interfaces.Game.Rooms.Services;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Db;
using Sadie.Db.Models.Players.Furniture;
using Sadie.Networking.Events.Attributes;
using Sadie.Networking.Writers.Rooms.Furniture;
using Sadie.Shared.Attributes;

namespace Sadie.Networking.Events.Handlers.Rooms.Furniture.Wired;

[PacketId(EventHandlerId.RoomWiredEffectSaved)]
public class RoomWiredEffectSavedEventHandler(
    IDbContextFactory<SadieDbContext> dbContextFactory,
    IRoomWiredService wiredService) : INetworkPacketEventHandler
{
    public required int ItemId { get; init; }
    public required List<int> Parameters { get; init; }
    public required string Input { get; init; }
    public required List<int> ItemIds { get; init; }
    public required int Delay { get; init; }
    public required int SelectionCode { get; init; }
    
    [RequiresRoomRights] 
    public async Task HandleAsync(INetworkClient client)
    {
        var room = client.RoomUser?.Room;

        var roomItem = room?.FurnitureItems
            .FirstOrDefault(x => x.Id == ItemId);

        if (roomItem == null)
        {
            return;
        }

        var selectedItems = room!
            .FurnitureItems
            .Where(x => ItemIds.Contains(x.Id))
            .ToList();

        await wiredService.SaveSettingsAsync(
            roomItem,
            dbContextFactory,
            new PlayerFurnitureItemWiredData
            {
                PlayerFurnitureItemPlacementDataId = roomItem.Id,
                PlacementData = roomItem,
                SelectedItems = selectedItems,
                Message = Input,
                Delay = Delay
            });

        await client.WriteToStreamAsync(new WiredSavedWriter());
    }
}