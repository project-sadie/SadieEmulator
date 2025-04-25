using Sadie.API;
using Sadie.API.Game.Rooms;
using Sadie.API.Networking;
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
    public required long RoomUserId { init; get; }
    public required string Height { init; get; }
    public required string NextHeight { init; get; }

    public override void OnSerialize(INetworkPacketWriter writer)
    {
        writer.WriteInteger(X);
        writer.WriteInteger(Y);
        writer.WriteInteger(NextX);
        writer.WriteInteger(NextY);
        writer.WriteInteger(0);
        writer.WriteInteger(RollerId);
        writer.WriteInteger(MovementType);
        writer.WriteLong(RoomUserId);
        writer.WriteString(Height);
        writer.WriteString(NextHeight);
    }
}