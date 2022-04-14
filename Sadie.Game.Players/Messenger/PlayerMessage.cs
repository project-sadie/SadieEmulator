namespace Sadie.Game.Players.Messenger;

public class PlayerMessage
{
    public int OriginId { get; }
    public int TargetId { get; }
    public string Message { get; }
    public DateTime CreatedAt { get; }

    public PlayerMessage(int originId,
        int targetId,
        string message)
    {
        OriginId = originId;
        TargetId = targetId;
        Message = message;
        CreatedAt = DateTime.Now;
    }
}