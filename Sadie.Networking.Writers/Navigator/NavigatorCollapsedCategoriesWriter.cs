using Sadie.API.Networking;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Navigator;

[PacketId(ServerPacketId.NavigatorCollapsedCategories)]
public class NavigatorCollapsedCategoriesWriter : AbstractPacketWriter
{
    public required List<string> Categories { get; init; }
}