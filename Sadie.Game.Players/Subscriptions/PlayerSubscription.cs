namespace Sadie.Game.Players.Subscriptions;

public class PlayerSubscription(string name, DateTime started, DateTime expires) : IPlayerSubscription
{
    public string Name { get; } = name;
    public DateTime Started { get; } = started;
    public DateTime Expires { get; } = expires;
}