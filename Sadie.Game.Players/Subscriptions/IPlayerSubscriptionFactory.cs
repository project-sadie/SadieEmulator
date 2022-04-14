namespace Sadie.Game.Players.Subscriptions;

public interface IPlayerSubscriptionFactory
{
    IPlayerSubscription Create(string name, DateTime started, DateTime expires);
}