using Sadie.Database;

namespace Sadie.Game.Players;

public class PlayerFactory
{
    public static PlayerBalance CreateBalanceFromRecord(DatabaseRecord record)
    {
        return new PlayerBalance(
            record.Get<long>("credit_balance"),
            record.Get<long>("pixel_balance"),
            record.Get<long>("seasonal_balance"),
            record.Get<long>("gotw_points"));
    }
    
    public static IPlayer CreateFromRecord(DatabaseRecord record)
    {
        return new Player(
            record.Get<long>("id"),
            record.Get<string>("username"),
            record.Get<long>("home_room_id"),
            record.Get<string>("figure_code"),
            record.Get<string>("motto"),
            record.Get<char>("gender") == 'M' ? PlayerAvatarGender.Male : PlayerAvatarGender.Female,
            CreateBalanceFromRecord(record),
            DateTime.TryParse(record.Get<string>("last_online"), out var timestamp) ? timestamp : DateTime.MinValue
        );
    }
}