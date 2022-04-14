namespace Sadie.Game.Players.Subscriptions;

public interface IPlayerSubscriptionDao
{
    Task<List<IPlayerSubscription>> GetSubscriptionsForPlayerAsync(int playerId);
}