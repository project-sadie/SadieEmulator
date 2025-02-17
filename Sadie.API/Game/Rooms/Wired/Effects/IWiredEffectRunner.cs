using Sadie.API.Game.Rooms.Users;
using Sadie.Database.Models.Players.Furniture;

namespace Sadie.API.Game.Rooms.Wired.Effects;

public interface IWiredEffectRunner
{
    public Task ExecuteAsync(
        IRoomLogic room,
        IRoomUser userWhoTriggered,
        PlayerFurnitureItemPlacementData effect);
}