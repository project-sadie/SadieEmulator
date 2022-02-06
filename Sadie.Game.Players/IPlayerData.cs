namespace Sadie.Game.Players;

public interface IPlayerData : IPlayerAvatarData
{
    long Id { get; }
    string Username { get; }
    long HomeRoom { get; }
    PlayerBalance Balance { get; }
    DateTime LastOnline { get; }
    public long RespectsReceived { get; }
    public long RespectPoints { get; }
    public long RespectPointsPet { get; }
}