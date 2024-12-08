using Sadie.Shared.Attributes;

namespace Sadie.API.Game.Rooms;

public interface IRoomRollingObjectData
{
    [PacketData] int Id { init; get; }
    [PacketData] string Height { init; get; }
    [PacketData] string NextHeight { init; get; }
}