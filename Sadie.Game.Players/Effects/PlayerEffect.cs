using Sadie.API.Game.Players.Effects;

namespace Sadie.Game.Players.Effects;

public class PlayerEffect(int duration, int id) : IPlayerEffect
{
    public int Id { get; } = id;
    public int Duration { get; } = duration;
}