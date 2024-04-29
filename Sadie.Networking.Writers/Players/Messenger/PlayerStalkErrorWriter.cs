using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Players.Messenger;

[PacketId(ServerPacketId.PlayerStalkError)]
public class PlayerStalkErrorWriter : AbstractPacketWriter
{
    public required int StalkError { get; init; }
}