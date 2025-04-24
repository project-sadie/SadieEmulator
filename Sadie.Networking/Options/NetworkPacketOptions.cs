namespace Sadie.Networking.Options;

public class NetworkPacketOptions
{
    public required int BufferByteSize { get; init; }
    public required int FrameLengthByteCount { get; init; }
}
