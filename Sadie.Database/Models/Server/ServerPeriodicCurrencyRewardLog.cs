using Sadie.Database.Models.Players;

namespace Sadie.Database.Models.Server;

public class ServerPeriodicCurrencyRewardLog
{
    public int Id { get; init; }
    public int PlayerId { get; init; }
    public Player Player { get; init; }
    public string? Type { get; init; }
    public int Amount { get; init; }
    public DateTime CreatedAt { get; init; }
}