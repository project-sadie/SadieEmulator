using Sadie.Database;

namespace Sadie.Game.Players;

public class PlayerFactory
{
    public static IPlayer CreateFromRecord(DatabaseRecord record)
    {
        return new Player(
            record.Get<long>("id"),
            record.Get<string>("username"),
            record.Get<long>("home_room_id"),
            record.Get<string>("figure_code"),
            record.Get<string>("motto"),
            record.Get<char>("gender") == 'M' ? PlayerAvatarGender.Male : PlayerAvatarGender.Female,
            new PlayerBalance(3333, 4444, 5555, 6666),
            DateTime.TryParse(record.Get<string>("last_online"), out var timestamp) ? timestamp : DateTime.MinValue
        );
    }
}