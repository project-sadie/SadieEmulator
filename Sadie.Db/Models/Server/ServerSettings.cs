namespace Sadie.Db.Models.Server;

public class ServerSettings
{
    public string? PlayerWelcomeMessage { get; init; }
    public bool FairCurrencyRewards { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}