using Sadie.API.DTOs.Player;
using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Packets.Writers.Players.Navigator;

[PacketId(ServerPacketId.NavigatorSettings)]
public class PlayerNavigatorSettingsWriter : AbstractPacketWriter
{
    public required PlayerNavigatorSettingsDto NavigatorSettings { get; init; }
}