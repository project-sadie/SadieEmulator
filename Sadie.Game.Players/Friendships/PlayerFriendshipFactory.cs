using Sadie.Database;

namespace Sadie.Game.Players.Friendships;

public static class PlayerFriendshipFactory
{
    public static PlayerFriendshipData CreateFromRecord(DatabaseRecord @record)
    {
        return new PlayerFriendshipData(
            record.Get<long>("id"),
            record.Get<string>("username"),
            record.Get<string>("figure_code"));
    }
}