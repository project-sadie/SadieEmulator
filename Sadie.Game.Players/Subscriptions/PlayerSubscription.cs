namespace Sadie.Game.Players.Subscriptions;

public class PlayerSubscription : IPlayerSubscription
{
    public string Name { get; }
    public DateTime Started { get; }
    public DateTime Expires { get; }
    
    public PlayerSubscription(string name, DateTime started, DateTime expires)
    {
        Name = name;
        Started = started;
        Expires = expires;
    }
}