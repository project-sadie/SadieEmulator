namespace Sadie.Database.Models.Server;

public class ServerPeriodicCurrencyReward
{
    public int Id { get; init; }
    public string? Type { get; init; }
    public int Amount { get; init; }
    public int IntervalSeconds { get; init; }
    public bool SkipIdle { get; init; }
    public bool SkipHotelView { get; init; }
}