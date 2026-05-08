using Sadie.API.DTOs.Rooms;
using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Writers.Navigator;

[PacketId(ServerPacketId.RoomCategories)]
public class RoomCategoriesWriter : AbstractPacketWriter
{
    public required List<RoomCategoryDto> Categories { get; init; }
}