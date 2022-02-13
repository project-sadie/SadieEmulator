namespace Sadie.Game.Rooms;

public static class RoomHelpers
{
    public static List<RoomTile> BuildTileListFromHeightMap(List<string> heightmapLines)
    {
        var tiles = new List<RoomTile>();
        
        for (var y = 0; y < heightmapLines.Count; y++)
        {
            var currentLine = heightmapLines[y];

            for (var x = 0; x < currentLine.Length; x++)
            {
                var zResult = int.TryParse(currentLine[x].ToString(), out var z);
                
                var state = zResult ? RoomTileState.Open : RoomTileState.Closed;
                var tile = new RoomTile(x, y, zResult ? z : 33, state);
                
                tiles.Add(tile);
            }
        }
        
        return tiles;
    }
}