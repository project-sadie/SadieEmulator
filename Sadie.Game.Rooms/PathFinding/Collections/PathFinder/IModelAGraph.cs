namespace Sadie.Game.Rooms.PathFinding.Collections.PathFinder
{
    internal interface IModelAGraph<T>
    {
        bool HasOpenNodes { get; }
        IEnumerable<T> GetSuccessors(T node);
        T GetParent(T node);
        void OpenNode(T node);
        T GetOpenNodeWithSmallestF();
    }
}