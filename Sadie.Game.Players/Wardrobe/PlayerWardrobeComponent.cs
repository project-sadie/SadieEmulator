namespace Sadie.Game.Players.Wardrobe;

public class PlayerWardrobeComponent(Dictionary<int, PlayerWardrobeItem> wardrobeItems)
{
    public Dictionary<int, PlayerWardrobeItem> WardrobeItems = wardrobeItems;
}