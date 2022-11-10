using Sadie.Database;

namespace Sadie.Game.Players.Respect;

public class PlayerRespectDao : BaseDao, IPlayerRespectDao
{
    public PlayerRespectDao(IDatabaseProvider databaseProvider) : base(databaseProvider)
    {
    }

    public async Task CreateAsync(int originId, int targetId)
    {
        await QueryAsync("INSERT INTO player_respects (origin_player_id,target_player_id, created_at) VALUES (@originId, @targetId, @createdAt);", new Dictionary<string, object>()
        {
            { "originId", originId },
            { "targetId", targetId },
            { "createdAt", DateTime.Now }
        });
    }
}