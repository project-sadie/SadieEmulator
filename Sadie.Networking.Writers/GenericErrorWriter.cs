using Sadie.Shared.Unsorted;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers;

public class GenericErrorWriter : NetworkPacketWriter
{
    public GenericErrorWriter(GenericErrorCode errorCode)
    {
        WriteShort(ServerPacketId.GenericError);
        WriteInteger((int) errorCode);
    }
}