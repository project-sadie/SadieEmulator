using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Navigator;

[PacketId(ServerPacketId.NavigatorCollapsedCategories)]
public class NavigatorCollapsedCategoriesWriter : AbstractPacketWriter
{
    public required List<string> Categories { get; init; }
}