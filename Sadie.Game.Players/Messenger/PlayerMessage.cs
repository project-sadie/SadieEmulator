namespace Sadie.Game.Players.Messenger;

public class PlayerMessage(
    int originId,
    int targetId,
    string message)
{
    public int OriginId { get; } = originId;
    public int TargetId { get; } = targetId;
    public string Message { get; } = message;
    public DateTime CreatedAt { get; } = DateTime.Now;
}