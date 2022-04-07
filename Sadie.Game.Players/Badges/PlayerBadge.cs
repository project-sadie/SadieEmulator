namespace Sadie.Game.Players.Badges;

public class PlayerBadge
{
    public int Id { get; }
    public string Code { get; }
    public int Slot { get; }
    
    public PlayerBadge(int id, string code, int slot)
    {
        Id = id;
        Code = code;
        Slot = slot;
    }
}