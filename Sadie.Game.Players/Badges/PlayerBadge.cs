namespace Sadie.Game.Players.Badges;

public class PlayerBadge(int id, string code, int slot)
{
    public int Id { get; } = id;
    public string Code { get; } = code;
    public int Slot { get; } = slot;
}