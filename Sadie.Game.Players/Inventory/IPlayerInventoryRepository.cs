namespace Sadie.Game.Players.Inventory;

public interface IPlayerInventoryRepository
{
    ICollection<PlayerInventoryFurnitureItem> Items { get; }
}