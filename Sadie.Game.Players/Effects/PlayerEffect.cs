namespace Sadie.Game.Players.Effects;

public class PlayerEffect(int duration, int id, DateTime activatedAt)
{
    public int Id { get; } = id;
    public int Duration { get; } = duration;
    public DateTime ActivatedAt { get; } = activatedAt;
}