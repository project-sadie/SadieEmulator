using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Packets.Writers.Rooms;

[PacketId(ServerPacketId.RoomOwner)] 
public class RoomOwnerWriter : AbstractPacketWriter; 