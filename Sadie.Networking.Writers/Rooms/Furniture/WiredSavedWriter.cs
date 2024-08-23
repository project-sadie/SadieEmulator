using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Rooms.Furniture;

[PacketId(ServerPacketId.WiredSaved)] 
public class WiredSavedWriter : AbstractPacketWriter;