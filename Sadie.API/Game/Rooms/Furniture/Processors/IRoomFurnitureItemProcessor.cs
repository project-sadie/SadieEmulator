using Sadie.Networking.Serialization;

namespace Sadie.API.Game.Rooms.Furniture.Processors;

public interface IRoomFurnitureItemProcessor
{
    Task<IEnumerable<AbstractPacketWriter>> GetUpdatesForRoomAsync(IRoomLogic roomLogic);
}