using Sadie.Game.Players.Badges;
using Sadie.Game.Players.Balance;
using Sadie.Game.Players.Navigator;
using Sadie.Shared.Game.Avatar;

namespace Sadie.Game.Players;

public interface IPlayerData : IAvatarData
{
    int Id { get; }
    string Username { get; }
    DateTime CreatedAt { get; }
    long HomeRoom { get; }
    IPlayerBalance Balance { get; }
    DateTime LastOnline { get; }
    int RespectsReceived { get; set; }
    int RespectPoints { get; set; }
    int RespectPointsPet { get; }
    PlayerNavigatorSettings NavigatorSettings { get; set; }
    PlayerSettings Settings { get; }
    List<PlayerSavedSearch> SavedSearches { get; }
    List<string> Permissions { get; }
    List<PlayerBadge> Badges { get; }
}