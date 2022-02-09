namespace Sadie.Networking.Packets.Server.Players;

internal class PlayerNotificationSettingsWriter : NetworkPacketWriter
{
    internal PlayerNotificationSettingsWriter(bool showNotifications) : base(ServerPacketId.NotificationSettings)
    {
        WriteBoolean(showNotifications);
    }
}