namespace Sadie.Game.Rooms.PathFinding.ToGo.Collections.PathFinder;

internal class ComparePathFinderNodeByFValue : IComparer<PathFinderNode>
{
    public int Compare(PathFinderNode a, PathFinderNode b)
    {
        if (a.F > b.F)
        {
            return 1;
        }

        if (a.F < b.F)
        {
            return -1;
        }
            
        return 0;
    }
}