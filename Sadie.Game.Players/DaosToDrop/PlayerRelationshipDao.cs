using Sadie.Database.LegacyAdoNet;
using Sadie.Database.Models.Players;
using Sadie.Game.Players.Relationships;

namespace Sadie.Game.Players.DaosToDrop;

public class PlayerRelationshipDao(IDatabaseProvider databaseProvider) : BaseDao(databaseProvider)
{
    public async Task<List<PlayerRelationship>> GetRelationshipsAsync(long playerId)
    {
        var reader = await GetReaderAsync(@"
            SELECT id, origin_player_id, target_player_id, type_id FROM player_relationships WHERE origin_player_id = @playerId", 
            new Dictionary<string, object>
            {
                { "playerId", playerId }
            });
        
        var data = new List<PlayerRelationship>();
        
        while (true)
        {
            var (success, record) = reader.Read();

            if (!success || record == null)
            {
                break;
            }
            
            data.Add(new PlayerRelationship());
        }

        return data;
    }

    public async Task UpdateRelationshipTypeAsync(int id, PlayerRelationshipType type)
    {
        await QueryAsync("UPDATE player_relationships SET type_id = @type WHERE id = @id", new Dictionary<string, object>
        {
            { "type", (int) type },
            { "id", id }
        });
    }

    public async Task<PlayerRelationship> CreateRelationshipAsync(long playerId, long targetPlayerId, PlayerRelationshipType type)
    {
        var parameters = new Dictionary<string, object>
        {
            { "playerId", playerId },
            { "targetPlayerId", targetPlayerId },
            { "type", (int) type }
        };

        var id = await QueryScalarAsync(@"
            INSERT INTO player_relationships (
                origin_player_id, target_player_id, type_id
            ) VALUES (@playerId, @targetPlayerId, @type); SELECT LAST_INSERT_ID();", parameters);

        return new PlayerRelationship();
    }

    public async Task DeleteRelationshipAsync(int id)
    {
        await QueryAsync("DELETE FROM player_relationships WHERE id = @id", new Dictionary<string, object>()
        {
            {"id", id}
        });
    }
}