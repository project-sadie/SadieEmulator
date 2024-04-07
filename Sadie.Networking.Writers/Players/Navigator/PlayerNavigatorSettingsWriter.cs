using Sadie.Database.Models.Players;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Navigator;

public class PlayerNavigatorSettingsWriter : NetworkPacketWriter
{
    public PlayerNavigatorSettingsWriter(PlayerNavigatorSettings navigatorSettings)
    {
        WriteShort(ServerPacketId.NavigatorSettings);
        WriteInteger(navigatorSettings.WindowX);
        WriteInteger(navigatorSettings.WindowY);
        WriteInteger(navigatorSettings.WindowWidth);
        WriteInteger(navigatorSettings.WindowHeight);
        WriteBool(navigatorSettings.OpenSearches);
        WriteInteger(navigatorSettings.Mode);
    }
}