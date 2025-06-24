using Sadie.API.Game.Rooms.Services;
using Sadie.API.Game.Rooms.Users;
using Sadie.Database.Models.Players.Furniture;

namespace Sadie.API.Game.Rooms.Wired.Effects.Actions;

public interface IWiredEffectAction
{
    public string InteractionType { get; }
    
    public Task ExecuteAsync(
        IRoomLogic room,
        IRoomUser userWhoTriggered,
        PlayerFurnitureItemPlacementData effect,
        IRoomWiredService wiredService);
}
