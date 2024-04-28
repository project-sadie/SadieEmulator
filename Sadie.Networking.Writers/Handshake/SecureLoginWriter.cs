using Sadie.Networking.Serialization;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets.Attributes;

namespace Sadie.Networking.Writers.Handshake;

[PacketId(ServerPacketId.SecureLogin)]
public class SecureLoginWriter : AbstractPacketWriter;