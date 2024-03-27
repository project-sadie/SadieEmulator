namespace Sadie.Shared.Unsorted.Game.Rooms;

public class HPoint
{
    public int X { get; }
    public int Y { get; }
    public float Z { get; }
    
    public HPoint(int x, int y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }
}