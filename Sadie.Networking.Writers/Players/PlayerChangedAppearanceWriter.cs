using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Players;

[PacketId(ServerPacketId.PlayerChangedAppearance)]
public class PlayerChangedAppearanceWriter : AbstractPacketWriter
{
    public required string FigureCode { get; init; }
    public required string Gender { get; init; }
}