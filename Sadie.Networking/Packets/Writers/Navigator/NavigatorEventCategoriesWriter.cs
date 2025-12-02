using Sadie.API.DTOs.Rooms;
using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Packets.Writers.Navigator;

[PacketId(ServerPacketId.NavigatorEventCategories)]
public class NavigatorEventCategoriesWriter : AbstractPacketWriter
{
    public required List<RoomCategoryDto> Categories { get; init; }
}