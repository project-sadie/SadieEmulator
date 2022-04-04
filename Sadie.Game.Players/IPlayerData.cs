using Sadie.Game.Players.Balance;
using Sadie.Game.Players.Navigator;
using Sadie.Shared.Game.Avatar;

namespace Sadie.Game.Players;

public interface IPlayerData : IAvatarData
{
    long Id { get; }
    string Username { get; }
    DateTime CreatedAt { get; }
    long HomeRoom { get; }
    IPlayerBalance Balance { get; }
    DateTime LastOnline { get; }
    long RespectsReceived { get; }
    long RespectPoints { get; }
    long RespectPointsPet { get; }
    PlayerNavigatorSettings NavigatorSettings { get; set; }
    PlayerSettings Settings { get; }
    List<PlayerSavedSearch> SavedSearches { get; }
    List<string> Permissions { get; }
}