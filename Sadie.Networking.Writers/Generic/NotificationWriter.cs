using Sadie.Shared;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Generic;

public class NotificationWriter : NetworkPacketWriter
{
    public NotificationWriter(NotificationType type, Dictionary<string, string> messages)
    {
        WriteShort(ServerPacketId.NotificationWriter);
        WriteString(type.ToString());
        WriteInteger(messages.Count);
        
        foreach (var pair in messages)
        {
            WriteString(pair.Key);
            WriteString(pair.Value);
        }
    }
}