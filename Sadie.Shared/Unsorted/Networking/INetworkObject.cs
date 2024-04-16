using DotNetty.Buffers;

namespace Sadie.Shared.Unsorted.Networking;

public interface INetworkObject
{
    Task WriteToStreamAsync(IByteBuffer data);
}