using Sadie.Game.Players.Avatar;

namespace Sadie.Game.Players;

public interface IPlayerData : IPlayerAvatarData
{
    long Id { get; }
    string Username { get; }
    long HomeRoom { get; }
    PlayerBalance Balance { get; }
    DateTime LastOnline { get; }
    long RespectsReceived { get; }
    long RespectPoints { get; }
    long RespectPointsPet { get; }
    PlayerNavigatorSettings NavigatorSettings { get; }
    PlayerSettings Settings { get; }
}