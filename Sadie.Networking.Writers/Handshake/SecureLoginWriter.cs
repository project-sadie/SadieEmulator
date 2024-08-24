using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Handshake;

[PacketId(ServerPacketId.SecureLogin)]
public class SecureLoginWriter : AbstractPacketWriter;