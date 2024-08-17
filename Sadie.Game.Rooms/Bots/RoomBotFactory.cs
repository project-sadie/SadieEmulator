using System.Drawing;
using Microsoft.Extensions.DependencyInjection;
using Sadie.API.Game.Rooms;
using Sadie.Database.Models.Rooms;

namespace Sadie.Game.Rooms.Bots;

public class RoomBotFactory(IServiceProvider serviceProvider)
{
    public RoomBot Create(
        IRoomLogic room,
        int id, 
        Point point)
    {
        return ActivatorUtilities.CreateInstance<RoomBot>(
            serviceProvider,
            id,
            room,
            point);
    }
}