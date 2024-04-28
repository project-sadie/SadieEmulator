namespace Sadie.Networking.Serialization.Attributes;

public class PacketIdAttribute(short id) : Attribute
{
    public short Id { get; } = id;
}