using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Players;

public class PlayerNotificationSettingsWriter : NetworkPacketWriter
{
    public PlayerNotificationSettingsWriter(bool showNotifications)
    {
        WriteShort(ServerPacketId.NotificationSettings);
        WriteBool(showNotifications);
    }
}