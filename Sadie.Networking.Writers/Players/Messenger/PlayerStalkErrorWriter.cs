using Sadie.Networking.Serialization;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets.Attributes;

namespace Sadie.Networking.Writers.Players.Messenger;

[PacketId(ServerPacketId.PlayerStalkError)]
public class PlayerStalkErrorWriter : AbstractPacketWriter
{
    public required int StalkError { get; init; }
}