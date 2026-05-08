using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Writers.Players.Messenger;

[PacketId(ServerPacketId.PlayerStalkError)]
public class PlayerStalkErrorWriter : AbstractPacketWriter
{
    public required int StalkError { get; init; }
}