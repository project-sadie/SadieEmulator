using Sadie.API;
using Sadie.API.Networking;
using Sadie.Database.Models.Players;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Players.Navigator;

[PacketId(ServerPacketId.NavigatorSettings)]
public class PlayerNavigatorSettingsWriter : AbstractPacketWriter
{
    public required PlayerNavigatorSettings NavigatorSettings { get; init; }

    public override void OnSerialize(INetworkPacketWriter writer)
    {
        writer.WriteInteger(NavigatorSettings.Id);
        writer.WriteInteger(NavigatorSettings.PlayerId);
        writer.WriteInteger(NavigatorSettings.WindowX);
        writer.WriteInteger(NavigatorSettings.WindowY);
        writer.WriteInteger(NavigatorSettings.WindowWidth);
        writer.WriteInteger(NavigatorSettings.WindowHeight);
        writer.WriteBool(NavigatorSettings.OpenSearches);
    }
}