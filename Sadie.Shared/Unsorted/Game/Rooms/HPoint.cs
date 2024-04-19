namespace Sadie.Shared.Unsorted.Game.Rooms;

public class HPoint
{
    public int X { get; }
    public int Y { get; }
    public double Z { get; }
    
    public HPoint(int x, int y, double z)
    {
        X = x;
        Y = y;
        Z = z;
    }
}