namespace Sadie.Game.Players.Respect;

public interface IPlayerRespectDao
{
    Task CreateAsync(int originId, int targetId);
}