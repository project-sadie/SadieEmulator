namespace Sadie.Shared.Unsorted.Networking.Packets.Attributes;

public class PacketIdAttribute(short id) : Attribute
{
    public short Id { get; } = id;
}