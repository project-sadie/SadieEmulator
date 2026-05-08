using System.Reflection;
using Sadie.API;

namespace Sadie.Networking.Packets.Serialization;

internal class PacketMetadata
{
    public short Id;
    public PropertyInfo[] Properties = [];
    public Dictionary<PropertyInfo, Action<INetworkPacketWriter>> BeforeRules = new();
    public Dictionary<PropertyInfo, Action<INetworkPacketWriter>> InsteadRules = new();
    public Dictionary<PropertyInfo, Action<INetworkPacketWriter>> AfterRules = new();
    public Dictionary<PropertyInfo, KeyValuePair<Type, Func<object, object>>> ConversionRules = new();
    public MethodInfo? OnConfigureAsync;
    public MethodInfo? OnSerializeAsync;
}