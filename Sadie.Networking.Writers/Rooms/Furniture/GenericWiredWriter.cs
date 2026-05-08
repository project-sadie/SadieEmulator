using Sadie.API.Interfaces.Networking;

namespace Sadie.Networking.Writers.Rooms.Furniture;

public class GenericWiredWriter : AbstractPacketWriter
{
    public required bool StuffTypeSelectionEnabled { get; init; }
    public required int MaxItemsSelected { get; init; }
    public required List<int> SelectedItemIds { get; init; }
}