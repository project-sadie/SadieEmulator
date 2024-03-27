using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;
using Sadie.Shared.Unsorted;

namespace Sadie.Networking.Writers;

public class GenericErrorWriter : NetworkPacketWriter
{
    public GenericErrorWriter(GenericErrorCode errorCode)
    {
        WriteShort(ServerPacketId.GenericError);
        WriteInteger((int) errorCode);
    }
}