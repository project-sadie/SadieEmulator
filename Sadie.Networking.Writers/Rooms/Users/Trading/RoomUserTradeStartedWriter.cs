using Sadie.API;
using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Writers.Rooms.Users.Trading;

[PacketId(ServerPacketId.RoomUserTradeStarted)]
public class RoomUserTradeStartedWriter : AbstractPacketWriter
{
    public required List<long> UserIds { get; init; }
    public required int State { get; init; }

    public override void OnSerialize(INetworkPacketWriter writer)
    {
        foreach (var id in UserIds)
        {
            writer.WriteLong(id);
            writer.WriteInteger(State);
        }
    }
}