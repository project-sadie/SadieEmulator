using Sadie.Networking.Serialization;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets.Attributes;

namespace Sadie.Networking.Writers.Players;

[PacketId(ServerPacketId.PlayerChangedAppearance)]
public class PlayerChangedAppearanceWriter : AbstractPacketWriter
{
    public required string FigureCode { get; init; }
    public required string Gender { get; init; }
}