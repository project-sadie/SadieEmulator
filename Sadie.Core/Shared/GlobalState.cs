namespace Sadie.Core.Shared;

public class GlobalState
{
    public static Random Random { get; } = new();
    public static Version? Version { get; set; }
}