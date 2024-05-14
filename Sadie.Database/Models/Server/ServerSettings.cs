namespace Sadie.Database.Models.Server;

public class ServerSettings
{
    public string? PlayerWelcomeMessage { get; init; }
    public bool MakeCurrencyRewardsFair { get; init; }
    public DateTime CreatedAt { get; init; }
}