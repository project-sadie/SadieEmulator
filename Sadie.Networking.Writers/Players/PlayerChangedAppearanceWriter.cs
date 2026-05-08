using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Writers.Players;

[PacketId(ServerPacketId.PlayerChangedAppearance)]
public class PlayerChangedAppearanceWriter : AbstractPacketWriter
{
    public required string FigureCode { get; init; }
    public required string Gender { get; init; }
}