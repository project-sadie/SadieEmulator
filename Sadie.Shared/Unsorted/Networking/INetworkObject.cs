namespace Sadie.Shared.Unsorted.Networking;

public interface INetworkObject
{
    Task WriteToStreamAsync(byte[] data);
}