namespace Sadie.Game.Players;

public interface IPlayerProvider
{
    Task<IPlayer> GetPlayerFromSsoAsync(string sso);
}