namespace Sadie.Game.Players.Effects;

public class PlayerEffect(int duration, int id)
{
    public int Id { get; } = id;
    public int Duration { get; } = duration;
}