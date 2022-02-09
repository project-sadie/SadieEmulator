namespace Sadie.Game.Players.Effects;

public class PlayerEffect
{
    public int Id { get; }
    public int Duration { get; }
    public DateTime ActivatedAt { get; }
    
    public PlayerEffect(int duration, int id, DateTime activatedAt)
    {
        Duration = duration;
        Id = id;
        ActivatedAt = activatedAt;
    }
}