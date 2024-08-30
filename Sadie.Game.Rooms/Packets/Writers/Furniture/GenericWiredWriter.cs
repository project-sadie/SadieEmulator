using Sadie.API.Networking;

namespace Sadie.Game.Rooms.Packets.Writers.Furniture;

public class GenericWiredWriter : AbstractPacketWriter
{
    public required bool StuffTypeSelectionEnabled { get; init; }
    public required int MaxItemsSelected { get; init; }
    public required List<int> SelectedItemIds { get; init; }
}