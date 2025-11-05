namespace Sadie.Db.Models.Players;

public class PlayerSubscription
{
    public int Id { get; init; }
    public long PlayerId { get; init; }
    public Player? Player { get; init; }
    public int SubscriptionId { get; init; }
    public Subscription? Subscription { get; init; }
    public DateTimeOffset ExpiresAt { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}