using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Writers.Generic;

[PacketId(ServerPacketId.GenericError)]
public class GenericErrorWriter : AbstractPacketWriter
{
    public required int ErrorCode { get; init; }
}