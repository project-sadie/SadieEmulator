using System.Drawing;
using Sadie.Enums.Unsorted;

namespace Sadie.API.Game.Rooms.Bots;

public interface IRoomBotFactory
{
    IRoomBot Create(
        IRoomLogic room,
        int id,
        Point point,
        double pointZ,
        HDirection directionHead,
        HDirection direction);
}