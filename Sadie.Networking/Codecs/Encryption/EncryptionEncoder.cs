using DotNetty.Buffers;
using DotNetty.Common.Utilities;
using DotNetty.Handlers.Streams;
using DotNetty.Transport.Channels;
using Sadie.Networking.Encryption;

namespace Sadie.Networking.Codecs.Encryption;

public class EncryptionEncoder(byte[] key) : ChunkedWriteHandler<object>
{
    private readonly Arc4 rc4 = new(key);

    public override Task WriteAsync(IChannelHandlerContext context, object message)
    {
        var input = (IByteBuffer)message;

        var data = input.ReadBytes(input.ReadableBytes);
        rc4.Parse(data.Array);

        ReferenceCountUtil.Release(input);

        return base.WriteAsync(context, data);
    }
}
