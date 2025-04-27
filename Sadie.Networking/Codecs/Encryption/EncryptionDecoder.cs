﻿using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Sadie.Networking.Encryption;

namespace Sadie.Networking.Codecs.Encryption;

public class EncryptionDecoder(byte[] key) : ByteToMessageDecoder
{
    private readonly Arc4 _rc4 = new(key);

    protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
    {
        var data = input.ReadBytes(input.ReadableBytes);
        _rc4.Parse(data.Array);
        output.Add(data);
    }
}
