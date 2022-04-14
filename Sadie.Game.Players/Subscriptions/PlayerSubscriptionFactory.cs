using Microsoft.Extensions.DependencyInjection;

namespace Sadie.Game.Players.Subscriptions;

public class PlayerSubscriptionFactory : IPlayerSubscriptionFactory
{
    private readonly IServiceProvider _serviceProvider;

    public PlayerSubscriptionFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public IPlayerSubscription Create(string name, DateTime started, DateTime expires)
    {
        return ActivatorUtilities.CreateInstance<PlayerSubscription>(
            _serviceProvider,
            name, 
            started, 
            expires);
    }
}