using Sadie.API.Networking;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Handshake;

[PacketId(ServerPacketId.SecureLogin)]
public class SecureLoginWriter : AbstractPacketWriter;