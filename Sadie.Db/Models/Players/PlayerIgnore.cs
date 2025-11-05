namespace Sadie.Db.Models.Players;

public class PlayerIgnore
{
    public int Id { get; init; }
    public required long PlayerId { get; init; }
    public required long TargetPlayerId { get; init; }
}