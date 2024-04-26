namespace Sadie.Shared.Unsorted.Networking.Packets.Attributes;

internal class PacketIdAttribute(short i) : Attribute
{
    public short Id { get; } = i;
}