using Sadie.Networking.Serialization;

namespace Sadie.Networking.Writers.Navigator;

public class NavigatorCollapsedCategoriesWriter : AbstractPacketWriter
{
    public required List<string> Categories { get; init; }
}