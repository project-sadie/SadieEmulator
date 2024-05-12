using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Players;

[PacketId(ServerPacketId.NotificationSettings)]
public class PlayerNotificationSettingsWriter : AbstractPacketWriter
{
    public required bool ShowNotifications { get; init; }
}