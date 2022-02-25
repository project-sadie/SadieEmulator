using Sadie.Game.Players;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Navigator;

public class PlayerNavigatorSettingsWriter : NetworkPacketWriter
{
    public PlayerNavigatorSettingsWriter(PlayerNavigatorSettings navigatorSettings) : base(ServerPacketId.NavigatorSettings)
    {
        WriteInt(navigatorSettings.WindowX);
        WriteInt(navigatorSettings.WindowY);
        WriteInt(navigatorSettings.WindowWidth);
        WriteInt(navigatorSettings.WindowHeight);
        WriteBoolean(navigatorSettings.OpenSearches);
        WriteInt(navigatorSettings.Unknown1);
    }
}