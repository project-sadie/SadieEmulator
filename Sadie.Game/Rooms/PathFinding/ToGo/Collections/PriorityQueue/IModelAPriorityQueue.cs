namespace Sadie.Game.Rooms.PathFinding.ToGo.Collections.PriorityQueue;

internal interface IModelAPriorityQueue<T>
{
    int Push(T item);
    T Pop();
    T Peek();
        
    void Clear();
    int Count { get; }
}