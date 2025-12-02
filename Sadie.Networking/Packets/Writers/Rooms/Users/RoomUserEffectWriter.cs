using Sadie.API;
using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Packets.Writers.Rooms.Users;

[PacketId(ServerPacketId.RoomUserEffect)]
public class RoomUserEffectWriter : AbstractPacketWriter
{
    public required long UserId { get; init; }
    public required int EffectId { get; init; }
    public required int DelayMs { get; init; }

    public override async Task OnSerializeAsync(INetworkPacketWriter writer)
    {
        writer.WriteLong(UserId);
        writer.WriteInteger(EffectId);
        writer.WriteInteger(DelayMs);
    }
}