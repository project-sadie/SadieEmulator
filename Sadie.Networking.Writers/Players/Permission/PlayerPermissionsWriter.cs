using Sadie.API.Networking;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Players.Permission;

[PacketId(ServerPacketId.PlayerPermissions)]
public class PlayerPermissionsWriter : AbstractPacketWriter
{
    public required int Club { get; init; }
    public required int Rank { get; init; }
    public required bool Ambassador { get; init; }
}