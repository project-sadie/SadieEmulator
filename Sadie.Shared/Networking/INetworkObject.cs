namespace Sadie.Shared.Networking;

public interface INetworkObject
{
    Task WriteToStreamAsync(byte[] data);
}