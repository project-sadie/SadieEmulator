namespace Sadie.Game.Players;

public class PlayerPerk(string code, string errorMessage, bool allowed)
{
    public string Code { get; set; } = code;
    public string ErrorMessage { get; set; } = errorMessage;
    public bool Allowed { get; set; } = allowed;
}