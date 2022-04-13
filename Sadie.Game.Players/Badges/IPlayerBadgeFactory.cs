namespace Sadie.Game.Players.Badges;

public interface IPlayerBadgeFactory
{
    PlayerBadge Create(int id, string code, int slot);
}