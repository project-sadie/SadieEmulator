using Sadie.Database.LegacyAdoNet;
using Sadie.Shared.Unsorted.Game.Avatar;

namespace Sadie.Game.Players.Wardrobe;

public class PlayerWardrobeDao : BaseDao, IPlayerWardrobeDao
{
    public PlayerWardrobeDao(IDatabaseProvider databaseProvider) : base(databaseProvider)
    {
        
    }
    
    public async Task<Dictionary<int, PlayerWardrobeItem>> GetAllRecordsForPlayerAsync(int playerId)
    {
        var reader = await GetReaderAsync(@"
            SELECT slot_id, figure_code, gender
            FROM player_wardrobe_items
            WHERE player_id = @playerId;", new Dictionary<string, object>
        {
            { "playerId", playerId }
        });

        var data = new Dictionary<int, PlayerWardrobeItem>();
        
        while (true)
        {
            var (success, record) = reader.Read();

            if (!success || record == null)
            {
                break;
            }

            data[record.Get<int>("slot_id")] = new PlayerWardrobeItem(
                record.Get<int>("slot_id"),
                record.Get<string>("figure_code"),
                record.Get<char>("gender") == 'M' ? AvatarGender.Male : AvatarGender.Female
            );
        }
        
        return data;
    }

    public async Task UpdateWardrobeItemAsync(long playerId, PlayerWardrobeItem wardrobeItem, bool newRecord)
    {
        var parameters = new Dictionary<string, object>()
        {
            { "playerId", playerId },
            { "slotId", wardrobeItem.SlotId },
            { "figureCode", wardrobeItem.FigureCode},
            { "gender", wardrobeItem.Gender == AvatarGender.Male ? "M" : "F" }
        };

        if (newRecord)
        {
            await QueryAsync(@"
            INSERT INTO player_wardrobe_items (player_id, slot_id, figure_code, gender) 
                VALUES (@playerId, @slotId, @figureCode, @gender)", parameters);
        }
        else
        {
            await QueryAsync(@"
            UPDATE player_wardrobe_items
                SET figure_code = @figureCode, gender = @gender 
                WHERE player_id = @playerId AND slot_id = @slotId", parameters);
        }
    }
}