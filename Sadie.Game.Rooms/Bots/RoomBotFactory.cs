using System.Drawing;
using Microsoft.Extensions.DependencyInjection;
using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Bots;

namespace Sadie.Game.Rooms.Bots;

public class RoomBotFactory(IServiceProvider serviceProvider) : IRoomBotFactory
{
    public IRoomBot Create(
        IRoomLogic room,
        int id, 
        Point point,
        double pointZ)
    {
        return ActivatorUtilities.CreateInstance<RoomBot>(
            serviceProvider,
            id,
            room,
            point,
            pointZ);
    }
}