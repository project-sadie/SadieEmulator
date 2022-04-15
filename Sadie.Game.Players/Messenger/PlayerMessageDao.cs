using Sadie.Database;

namespace Sadie.Game.Players.Messenger;

public class PlayerMessageDao : BaseDao, IPlayerMessageDao
{
    public PlayerMessageDao(IDatabaseProvider databaseProvider) : base(databaseProvider)
    {
    }
    
    public async Task<int> CreateMessageAsync(PlayerMessage message)
    {
        return await QueryAsync("INSERT INTO `player_messages` (`origin_player_id`, `target_player_id`, `message`, `sent_at`) VALUES (@originId, @targetId, @message, @sentAt);", new Dictionary<string, object>()
        {
            {"originId", message.OriginId},
            {"targetId", message.TargetId},
            {"message", message.Message},
            {"sentAt", message.CreatedAt},
        });
    }
}