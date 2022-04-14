namespace Sadie.Game.Players.Subscriptions;

public interface IPlayerSubscription
{
    string Name { get; }
    DateTime Started { get; }
    DateTime Expires { get; }
}