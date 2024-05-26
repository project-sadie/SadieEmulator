using Sadie.Database.Models.Rooms;
using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Navigator;

[PacketId(ServerPacketId.NavigatorEventCategories)]
public class NavigatorEventCategoriesWriter : AbstractPacketWriter
{
    public required List<RoomCategory> Categories { get; init; }
}