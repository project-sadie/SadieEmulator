using Microsoft.Extensions.DependencyInjection;
using Sadie.Game.Players.Friendships;

namespace Sadie.Game.Players;

public class PlayerServiceCollection
{
    public static void AddServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IPlayerBalance, PlayerBalance>();
        serviceCollection.AddSingleton<IPlayer, Player>();
        serviceCollection.AddSingleton<IPlayerFactory, PlayerFactory>();
        serviceCollection.AddSingleton<IPlayerDao, PlayerDao>();
        serviceCollection.AddSingleton<IPlayerRepository, PlayerRepository>();
        serviceCollection.AddSingleton<IPlayerFriendshipDao, PlayerFriendshipDao>();
        serviceCollection.AddSingleton<IPlayerFriendshipRepository, PlayerFriendshipRepository>();
    }
}