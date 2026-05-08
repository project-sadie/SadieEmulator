namespace Sadie.Core.Shared.Attributes;

[AttributeUsage(AttributeTargets.All)]
public class PacketIdAttribute(short id) : Attribute
{
    public short Id { get; } = id;
}