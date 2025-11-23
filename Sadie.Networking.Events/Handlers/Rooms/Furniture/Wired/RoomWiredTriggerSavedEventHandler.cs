using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sadie.API.DTOs.Player.Furniture;
using Sadie.API.Interfaces.Game.Rooms.Services;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Shared.Attributes;
using Sadie.Db;
using Sadie.Networking.Events.Attributes;
using Sadie.Networking.Writers.Rooms.Furniture;

namespace Sadie.Networking.Events.Handlers.Rooms.Furniture.Wired;

[PacketId(EventHandlerId.RoomWiredTriggerSaved)]
public class RoomWiredTriggerSavedEventHandler(
    IDbContextFactory<SadieDbContext> dbContextFactory,
    IRoomWiredService wiredService,
    IMapper mapper) : INetworkPacketEventHandler
{
    public required int ItemId { get; init; }
    public required List<int> Parameters { get; init; }
    public required string Input { get; init; }
    public required List<int> ItemIds { get; init; }
    public required int SelectionCode { get; init; }
    
    [RequiresRoomRights] 
    public async Task HandleAsync(INetworkClient client)
    {
        var room = client.RoomUser?.Room;

        var roomItem = room?
            .Room.FurnitureItems
            .FirstOrDefault(x => x.Id == ItemId);

        if (roomItem == null)
        {
            return;
        }

        var roomItems = room!
            .Room
            .FurnitureItems
            .Where(x => ItemIds.Contains(x.Id))
            .ToList();

        await wiredService.SaveSettingsAsync(
            roomItem,
            new PlayerFurnitureItemWiredDataDto
            {
                PlayerFurnitureItemPlacementDataId = roomItem.Id,
                PlacementData = roomItem,
                SelectedItems = roomItems,
                Message = Input
            });
        
        await client.WriteToStreamAsync(new WiredSavedWriter());
    }
}