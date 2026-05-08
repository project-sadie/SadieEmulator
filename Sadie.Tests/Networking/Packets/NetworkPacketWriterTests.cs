using Sadie.Networking.Packets;

namespace Sadie.Tests.Networking.Packets;

[TestFixture]
public class NetworkPacketWriterTests
{
    [Test]
    public void WriteInteger_Value_BigEndianBytes()
    {
        var writer = new NetworkPacketWriter();
        writer.WriteInteger(1);

        var bytes = writer.GetAllBytes();
        var intBytes = bytes.Skip(4).Take(4).ToArray();
        Assert.That(intBytes, Is.EqualTo(new byte[] { 0, 0, 0, 1 }));
    }

    [Test]
    public void WriteShort_Value_BigEndianBytes()
    {
        var writer = new NetworkPacketWriter();
        writer.WriteShort(1);

        var bytes = writer.GetAllBytes();
        var shortBytes = bytes.Skip(4).Take(2).ToArray();
        Assert.That(shortBytes, Is.EqualTo(new byte[] { 0, 1 }));
    }

    [Test]
    public void WriteString_Value_LengthPrefixedUtf8()
    {
        var writer = new NetworkPacketWriter();
        writer.WriteString("Hi");

        var bytes = writer.GetAllBytes();
        var payload = bytes.Skip(4).ToArray();
        
        Assert.Multiple(() =>
        {
            Assert.That(payload[0], Is.EqualTo(0));
            Assert.That(payload[1], Is.EqualTo(2));
            Assert.That(payload[2], Is.EqualTo((byte)'H'));
            Assert.That(payload[3], Is.EqualTo((byte)'i'));
        });
    }

    [Test]
    public void WriteBool_True_WritesOne()
    {
        var writer = new NetworkPacketWriter();
        writer.WriteBool(true);

        var bytes = writer.GetAllBytes();
        Assert.That(bytes[4], Is.EqualTo(1));
    }

    [Test]
    public void WriteBool_False_WritesZero()
    {
        var writer = new NetworkPacketWriter();
        writer.WriteBool(false);

        var bytes = writer.GetAllBytes();
        Assert.That(bytes[4], Is.EqualTo(0));
    }

    [Test]
    public void GetAllBytes_IncludesLengthPrefix()
    {
        var writer = new NetworkPacketWriter();
        writer.WriteInteger(42);

        var bytes = writer.GetAllBytes();
        Assert.That(bytes.Length, Is.EqualTo(8));
        
        var lengthPrefix = BitConverter.ToInt32(bytes.Take(4).Reverse().ToArray(), 0);
        Assert.That(lengthPrefix, Is.EqualTo(4));
    }
}
