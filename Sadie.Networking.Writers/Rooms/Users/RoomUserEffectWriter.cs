using Sadie.API;
using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Writers.Rooms.Users;

[PacketId(ServerPacketId.RoomUserEffect)]
public class RoomUserEffectWriter : AbstractPacketWriter
{
    public required long UserId { get; init; }
    public required int EffectId { get; init; }
    public required int DelayMs { get; init; }

    public override void OnSerialize(INetworkPacketWriter writer)
    {
        writer.WriteLong(UserId);
        writer.WriteInteger(EffectId);
        writer.WriteInteger(DelayMs);
    }
}