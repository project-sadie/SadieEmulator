using Sadie.Game.Players.Avatar;
using Sadie.Game.Players.Navigator;

namespace Sadie.Game.Players;

public interface IPlayerData : IPlayerAvatarData
{
    long Id { get; }
    string Username { get; }
    long HomeRoom { get; }
    IPlayerBalance Balance { get; }
    DateTime LastOnline { get; }
    long RespectsReceived { get; }
    long RespectPoints { get; }
    long RespectPointsPet { get; }
    PlayerNavigatorSettings NavigatorSettings { get; set; }
    PlayerSettings Settings { get; }
    List<PlayerSavedSearch> SavedSearches { get; }
}