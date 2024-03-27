using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;
using Sadie.Shared.Unsorted;

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