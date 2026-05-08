using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Writers.Handshake;

[PacketId(ServerPacketId.SecureLogin)]
public class SecureLoginWriter : AbstractPacketWriter;