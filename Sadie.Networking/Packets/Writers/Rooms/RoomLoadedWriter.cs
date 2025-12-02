using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Packets.Writers.Rooms;

[PacketId(ServerPacketId.RoomLoaded)]
public class RoomLoadedWriter : AbstractPacketWriter;