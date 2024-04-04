using Sadie.Database.LegacyAdoNet;

namespace Sadie.Game.Players.Respect;

public class PlayerRespectDao(IDatabaseProvider databaseProvider) : BaseDao(databaseProvider), IPlayerRespectDao
{
    public async Task CreateAsync(int originId, int targetId)
    {
        await QueryAsync(@"
            INSERT INTO player_respects (
                origin_player_id,
                target_player_id, 
                created_at
            ) 
            VALUES (@originId, @targetId, @createdAt);", new Dictionary<string, object>
        {
            { "originId", originId },
            { "targetId", targetId },
            { "createdAt", DateTime.Now }
        });
    }
}