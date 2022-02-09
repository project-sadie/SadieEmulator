using Sadie.Game.Players;

namespace Sadie.Networking.Packets.Server.Players.Navigator;

internal class PlayerNotificationSettingsWriter : NetworkPacketWriter
{
    internal PlayerNotificationSettingsWriter(bool showNotifications) : base(ServerPacketId.NotificationSettings)
    {
        WriteBoolean(showNotifications);
    }
}