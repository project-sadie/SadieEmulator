using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Rooms;

[PacketId(ServerPacketId.RoomOwner)] 
public class RoomOwnerWriter : AbstractPacketWriter; 