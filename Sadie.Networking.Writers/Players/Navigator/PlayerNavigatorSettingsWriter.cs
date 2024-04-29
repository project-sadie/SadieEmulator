using Sadie.Database.Models.Players;
using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Players.Navigator;

[PacketId(ServerPacketId.NavigatorSettings)]
public class PlayerNavigatorSettingsWriter : AbstractPacketWriter
{
    public required PlayerNavigatorSettings NavigatorSettings { get; init; }
}