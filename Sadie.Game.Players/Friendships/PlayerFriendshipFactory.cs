using Microsoft.Extensions.DependencyInjection;
using Sadie.Database;

namespace Sadie.Game.Players.Friendships;

public class PlayerFriendshipFactory
{
    private readonly IServiceProvider _serviceProvider;

    public PlayerFriendshipFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public PlayerFriendshipData CreateFromRecord(DatabaseRecord @record)
    {
        return ActivatorUtilities.CreateInstance<PlayerFriendshipData>(
            _serviceProvider, 
            record.Get<long>("id"),
            record.Get<string>("username"),
            record.Get<string>("figure_code"),
            (PlayerFriendshipType) record.Get<int>("type_id"));
    }
}