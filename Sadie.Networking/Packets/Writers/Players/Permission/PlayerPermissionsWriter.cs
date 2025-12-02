using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Packets.Writers.Players.Permission;

[PacketId(ServerPacketId.PlayerPermissions)]
public class PlayerPermissionsWriter : AbstractPacketWriter
{
    public required int Club { get; init; }
    public required int Rank { get; init; }
    public required bool Ambassador { get; init; }
}