using Sadie.Game.Players;

namespace Sadie.Networking.Packets.Server.Players.Navigator;

internal class PlayerNavigatorSettingsWriter : NetworkPacketWriter
{
    internal PlayerNavigatorSettingsWriter(PlayerNavigatorSettings navigatorSettings) : base(ServerPacketId.NavigatorSettings)
    {
        WriteInt(navigatorSettings.WindowX);
        WriteInt(navigatorSettings.WindowY);
        WriteInt(navigatorSettings.WindowWidth);
        WriteInt(navigatorSettings.WindowHeight);
        WriteBoolean(navigatorSettings.OpenSearches);
        WriteInt(navigatorSettings.Unknown1);
    }
}