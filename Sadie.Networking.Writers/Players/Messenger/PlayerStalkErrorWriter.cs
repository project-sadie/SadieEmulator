using Sadie.API.Networking;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Players.Messenger;

[PacketId(ServerPacketId.PlayerStalkError)]
public class PlayerStalkErrorWriter : AbstractPacketWriter
{
    public required int StalkError { get; init; }
}