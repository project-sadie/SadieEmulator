namespace Sadie.Game.Players;

public interface IPlayerData : IPlayerAvatarData
{
    long Id { get; }
    string Username { get; }
    long HomeRoom { get; }
}