using Sadie.Game.Players;

namespace Sadie.Networking.Packets.Server.Players.Navigator;

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