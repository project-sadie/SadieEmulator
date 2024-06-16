namespace Sadie.Shared;

public static class GlobalState
{
    public static readonly Version Version = new(0, 9);
    public static DateTime Started { get; set; }
    public static Random Random = new Random((int)DateTime.Now.Ticks);
}