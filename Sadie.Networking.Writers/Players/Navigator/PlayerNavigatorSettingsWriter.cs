using Sadie.API.Networking;
using Sadie.Database.Models.Players;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Players.Navigator;

[PacketId(ServerPacketId.NavigatorSettings)]
public class PlayerNavigatorSettingsWriter : AbstractPacketWriter
{
    public required PlayerNavigatorSettings NavigatorSettings { get; init; }
}