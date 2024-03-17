using Microsoft.Extensions.DependencyInjection;

namespace Sadie.Game.Players.Subscriptions;

public class PlayerSubscriptionFactory(IServiceProvider serviceProvider) : IPlayerSubscriptionFactory
{
    public IPlayerSubscription Create(string name, DateTime started, DateTime expires)
    {
        return ActivatorUtilities.CreateInstance<PlayerSubscription>(
            serviceProvider,
            name, 
            started, 
            expires);
    }
}