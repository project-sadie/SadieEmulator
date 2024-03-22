namespace Sadie.Game.Players.Wardrobe;

public interface IPlayerWardrobeDao
{
    Task<Dictionary<int, PlayerWardrobeItem>> GetAllRecordsForPlayerAsync(int playerId);
    Task UpdateWardrobeItemAsync(long playerId, PlayerWardrobeItem wardrobeItem);
}