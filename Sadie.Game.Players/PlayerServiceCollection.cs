using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sadie.Game.Players.Balance;
using Sadie.Game.Players.Club;
using Sadie.Game.Players.DaosToDrop;
using Sadie.Game.Players.Friendships;
using Sadie.Game.Players.Messenger;
using Sadie.Game.Players.Respect;
using Sadie.Game.Players.Room;

namespace Sadie.Game.Players;

public static class PlayerServiceCollection
{
    public static void AddServices(IServiceCollection serviceCollection, IConfiguration config)
    {
        serviceCollection.AddSingleton<IPlayerRoomVisitDao, PlayerRoomVisitDao>();
        serviceCollection.AddTransient<PlayerFriendshipComponent>();
        serviceCollection.AddTransient<IPlayerState, PlayerState>();
        serviceCollection.AddSingleton<IPlayerMessageDao, PlayerMessageDao>();
        serviceCollection.AddSingleton<IPlayerRespectDao, PlayerRespectDao>();
        serviceCollection.AddTransient<IPlayerBalance, PlayerBalance>();
        serviceCollection.AddTransient<PlayerLogic, PlayerLogic>();
        serviceCollection.AddSingleton<IPlayerFactory, PlayerFactory>();
        serviceCollection.AddSingleton<PlayerRepository>();
        serviceCollection.AddSingleton<IPlayerFriendshipDao, PlayerFriendshipDao>();
        serviceCollection.AddSingleton<IPlayerFriendshipRepository, PlayerFriendshipRepository>();
        serviceCollection.AddSingleton<PlayerFriendshipFactory>();
        
        var playerConstants = new PlayerConstants();
        config.GetSection("Constants:Player").Bind(playerConstants);
        serviceCollection.AddSingleton(playerConstants);
        
        serviceCollection.AddSingleton<PlayerClubOfferFactory>();
        serviceCollection.AddSingleton<PlayerClubOfferRepository>();
        serviceCollection.AddSingleton<PlayerClubOfferDao>();
        serviceCollection.AddTransient<PlayerClubOffer>();
    }
}