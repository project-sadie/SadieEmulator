using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking;

public class PacketEncoder : MessageToByteEncoder<NetworkPacketWriter>
{
    protected override void Encode(IChannelHandlerContext context, NetworkPacketWriter message, IByteBuffer output)
    {
        output.WriteBytes(message.GetAllBytes2());
    }
}