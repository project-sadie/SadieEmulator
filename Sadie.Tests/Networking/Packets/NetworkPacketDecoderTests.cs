using Sadie.Networking.Packets;

namespace Sadie.Tests.Networking.Packets;

[TestFixture]
public class NetworkPacketDecoderTests
{
    private NetworkPacketDecoder _decoder;

    [SetUp]
    public void SetUp()
    {
        _decoder = new NetworkPacketDecoder();
    }

    [Test]
    public void Decode_ValidPacket_ExtractsPacketId()
    {
        var buffer = new byte[] { 0, 0, 0, 7, 0, 100, 1, 2, 3 };

        var packet = _decoder.Decode(Guid.NewGuid(), buffer, buffer.Length);

        Assert.That(packet.PacketId, Is.EqualTo(100));
    }

    [Test]
    public void Decode_ValidPacket_ExtractsBody()
    {
        var buffer = new byte[] { 0, 0, 0, 7, 0, 50, 0xAA, 0xBB, 0xCC };

        var packet = _decoder.Decode(Guid.NewGuid(), buffer, buffer.Length);

        Assert.That(packet.Data.Length, Is.EqualTo(3));
        Assert.Multiple(() =>
        {
            Assert.That(packet.Data.Span[0], Is.EqualTo(0xAA));
            Assert.That(packet.Data.Span[1], Is.EqualTo(0xBB));
            Assert.That(packet.Data.Span[2], Is.EqualTo(0xCC));
        });
    }

    [Test]
    public void Decode_MinimalPacket_EmptyBody()
    {
        var buffer = new byte[] { 0, 0, 0, 2, 0, 1 };

        var packet = _decoder.Decode(Guid.NewGuid(), buffer, buffer.Length);
        Assert.Multiple(() =>
        {
            Assert.That(packet.PacketId, Is.EqualTo(1));
            Assert.That(packet.Data.Length, Is.EqualTo(0));
        });
    }
}
