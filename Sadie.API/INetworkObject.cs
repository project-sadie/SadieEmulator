using DotNetty.Transport.Channels;
using Sadie.API.Networking;

namespace Sadie.API;

public interface INetworkObject
{
    Task WriteToStreamAsync(AbstractPacketWriter writer);
    Task WriteToStreamAsync(INetworkPacketWriter writer);
    IChannel Channel { get; set; } 
}