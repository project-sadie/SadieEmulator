namespace Sadie.Database.Models.Players;

public class PlayerSubscription
{
    public int Id { get; set; }
    public int PlayerId { get; set; }
    public Player Player { get; set; }
    public int SubscriptionId { get; set; }
    public Subscription Subscription { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
}