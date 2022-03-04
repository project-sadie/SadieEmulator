using System.Drawing;
using Sadie.Shared;
using Sadie.Shared.Networking;

namespace Sadie.Game.Rooms.Users;

public interface IRoomUser : IDisposable
{
    INetworkObject NetworkObject { get; }
    string Username { get; }
    string Motto { get; }
    string FigureCode { get; }
    string Gender { get; }
    long AchievementScore { get; }
    long Id { get; }
    HPoint Point { get; }
    HDirection DirectionHead { get; }
    HDirection Direction { get; }
    void WalkToPoint(Point point, bool useDiagonal);
    Task RunPeriodicCheckAsync();
    void Dispose();
}