using System.Reflection;

// ReSharper disable CollectionNeverQueried.Global
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace Sadie.Shared.Unsorted.Networking.Packets;

public abstract class AbstractPacketWriter
{
    public Dictionary<PropertyInfo, Action<NetworkPacketWriter>> BeforeRulesSerialize { get; } = new();
    public Dictionary<PropertyInfo, Action<NetworkPacketWriter>> InsteadRulesSerialize { get; } = new();
    public Dictionary<PropertyInfo, Action<NetworkPacketWriter>> AfterRulesSerialize { get; } = new();
    
    public virtual void OnSerialize()
    {
        
    }

    protected void Before(PropertyInfo propertyInfo, Action<NetworkPacketWriter> function)
    {
        BeforeRulesSerialize.Add(propertyInfo, function);
    }

    protected void Override(PropertyInfo propertyInfo, Action<NetworkPacketWriter> function)
    {
        InsteadRulesSerialize.Add(propertyInfo, function);
    }

    protected void After(PropertyInfo propertyInfo, Action<NetworkPacketWriter> function)
    {
        AfterRulesSerialize.Add(propertyInfo, function);
    }
}