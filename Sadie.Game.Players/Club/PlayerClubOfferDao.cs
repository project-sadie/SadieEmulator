using Sadie.Database;

namespace Sadie.Game.Players.Club;

public class PlayerClubOfferDao(IDatabaseProvider databaseProvider, PlayerClubOfferFactory factory) : BaseDao(databaseProvider)
{
    public async Task<List<PlayerClubOffer>> GetAllAsync()
    {
        var offers = new List<PlayerClubOffer>();
        
        var reader = await GetReaderAsync(@"SELECT 
            id, 
            name, 
            duration_days, 
            cost_credits, 
            cost_points, 
            cost_points_type, 
            is_vip FROM player_club_offers");

        while (true)
        {
            var (success, record) = reader.Read();

            if (!success || record == null)
            {
                break;
            }

            var offer = factory.Create(
                record.Get<int>("id"),
                record.Get<string>("name"),
                record.Get<int>("duration_days"),
                record.Get<int>("cost_credits"),
                record.Get<int>("cost_points"),
                record.Get<int>("cost_points_type"),
                record.Get<int>("is_vip") == 1);
            
            offers.Add(offer);
        }

        return offers;
    }
}