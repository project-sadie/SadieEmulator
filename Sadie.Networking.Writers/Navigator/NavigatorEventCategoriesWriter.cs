using Sadie.Database.Models.Rooms;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;
using Sadie.Shared.Unsorted.Networking.Packets.Attributes;

namespace Sadie.Networking.Writers.Navigator;

[PacketId(ServerPacketId.NavigatorEventCategories)]
public class NavigatorEventCategoriesWriter : NetworkPacketWriter
{
    public required List<RoomCategory> Categories { get; init; }
}