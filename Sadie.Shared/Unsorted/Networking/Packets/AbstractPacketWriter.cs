using System.Reflection;

// ReSharper disable CollectionNeverQueried.Global
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace Sadie.Shared.Unsorted.Networking.Packets;

public abstract class AbstractPacketWriter : NetworkPacketWriter
{
    public Dictionary<PropertyInfo, Action<NetworkPacketWriter>> RulesForSerialize { get; } = new();
    
    public virtual void OnSerialize()
    {
        
    }

    protected void AddRule(PropertyInfo propertyInfo, Action<NetworkPacketWriter> function)
    {
        RulesForSerialize.Add(propertyInfo, function);
    }
}