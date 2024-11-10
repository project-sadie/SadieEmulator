using Sadie.API.Networking;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Rooms.Furniture;

[PacketId(ServerPacketId.WiredSaved)] 
public class WiredSavedWriter : AbstractPacketWriter;