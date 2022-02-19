namespace Sadie.Networking.Packets.Server.Players;

public class PlayerNotificationSettingsWriter : NetworkPacketWriter
{
    public PlayerNotificationSettingsWriter(bool showNotifications) : base(ServerPacketId.NotificationSettings)
    {
        WriteBoolean(showNotifications);
    }
}