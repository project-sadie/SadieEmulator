using Sadie.Database;

namespace Sadie.Game.Players;

public class PlayerFactory
{
    public static IPlayer CreateFromRecord(DatabaseRecord record)
    {
        return new Player(
            record.Get<long>("id"),
            record.Get<long>("home_room_id")
        );
    }
}