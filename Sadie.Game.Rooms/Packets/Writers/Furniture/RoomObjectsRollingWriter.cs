using Sadie.API.Game.Rooms;
using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers;

namespace Sadie.Game.Rooms.Packets.Writers.Furniture;

[PacketId(ServerPacketId.RoomObjectsRolling)]
public class RoomObjectsRollingWriter : AbstractPacketWriter
{
    public required int X { init; get; }
    public required int Y { init; get; }
    public required int NextX { init; get; }
    public required int NextY { init; get; }
    public required ICollection<IRoomRollingObjectData> Objects { init; get; }
    public required int RollerId { init; get; }
    public required int MovementType { init; get; }
    public required int RoomUserId { init; get; }
    public required string Height { init; get; }
    public required string NextHeight { init; get; }

    public override void OnConfigureRules()
    {
        Override(GetType().GetProperty(nameof(Objects))!, writer =>
        {
            writer.WriteInteger(Objects.Count);

            foreach (var roomRollingObjectData in Objects)
            {
                writer.WriteInteger(roomRollingObjectData.Id);
                writer.WriteString(roomRollingObjectData.Height);
                writer.WriteString(roomRollingObjectData.NextHeight);
            }
        });
    }
}