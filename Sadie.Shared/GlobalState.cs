namespace Sadie.Shared;

public static class GlobalState
{
    public static readonly Version Version = new(0, 9);
    public static readonly Random Random = new((int)DateTime.Now.Ticks);
}