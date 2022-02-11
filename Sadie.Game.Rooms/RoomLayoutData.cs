using System.Text;

namespace Sadie.Game.Rooms;

public class RoomLayoutData
{
    public List<string> HeightmapRows { get; }
    public int SizeX { get; }
    public int SizeY { get; }
    public string RelativeHeightmap { get; }

    protected RoomLayoutData(string heightmap)
    {
        HeightmapRows = heightmap.Split("\n").ToList();
        SizeX = HeightmapRows.OrderByDescending(x => x.Length).First().Length;
        SizeY = HeightmapRows.Count;
        RelativeHeightmap = BuildRelativeHeightMap();
    }

    private string BuildRelativeHeightMap()
    {
        // return string.Join('\r', Enumerable.Repeat(new string('0', SizeX), SizeY));
        
        var sb = new StringBuilder();
    
        for (var y = 0; y < SizeY; y++)
        {
            sb.Append(new string('0', SizeX) + '\r');
        }

        return sb.ToString();
    }
}