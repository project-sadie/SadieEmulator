using Sadie.Networking.Serialization;
using Sadie.Shared.Unsorted;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Generic;

public class NotificationWriter : AbstractPacketWriter
{
    public NotificationWriter(
        NotificationType type, 
        Dictionary<string, string> messages)
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