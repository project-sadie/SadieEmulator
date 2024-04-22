using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Generic;

public class ServerErrorWriter : NetworkPacketWriter
{
    public ServerErrorWriter(int messageId, int errorCode)
    {
        WriteShort(ServerPacketId.ServerError);
        WriteInteger(messageId);
        WriteInteger(errorCode);
        WriteString(DateTime.Now.ToString("M/d/yy, h:mm tt"));
    }
}