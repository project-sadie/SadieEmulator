using Sadie.API.Networking;
using Sadie.Database.Models.Rooms;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Navigator;

[PacketId(ServerPacketId.NavigatorEventCategories)]
public class NavigatorEventCategoriesWriter : AbstractPacketWriter
{
    public required List<RoomCategory> Categories { get; init; }
}