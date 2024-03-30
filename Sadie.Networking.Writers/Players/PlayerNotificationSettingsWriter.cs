using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Players;

public class PlayerNotificationSettingsWriter : NetworkPacketWriter
{
    public PlayerNotificationSettingsWriter(bool showNotifications)
    {
        WriteShort(ServerPacketId.NotificationSettings);
        WriteBool(showNotifications);
    }
}