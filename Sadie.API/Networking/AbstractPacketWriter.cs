using System.Reflection;

// ReSharper disable CollectionNeverQueried.Global
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace Sadie.API.Networking;

public abstract class AbstractPacketWriter
{
    public Dictionary<PropertyInfo, Action<INetworkPacketWriter>> InsteadRulesSerialize { get; } = new();
    public Dictionary<PropertyInfo, Action<INetworkPacketWriter>> AfterRulesSerialize { get; } = new();
    public Dictionary<PropertyInfo, KeyValuePair<Type, Func<object, object>>> ConversionRules { get; } = new();
    
    /// <summary>
    /// Register any rules needed for custom mapping
    /// </summary>
    public virtual void OnConfigureRules()
    {
        
    }

    /// <summary>
    /// Overrides the entire serialization process
    /// </summary>
    public virtual void OnSerialize(INetworkPacketWriter writer)
    {
        
    }

    protected void Override(PropertyInfo propertyInfo, Action<INetworkPacketWriter> function)
    {
        InsteadRulesSerialize.Add(propertyInfo, function);
    }

    protected void After(PropertyInfo propertyInfo, Action<INetworkPacketWriter> function)
    {
        AfterRulesSerialize.Add(propertyInfo, function);
    }

    protected void Convert<TType>(PropertyInfo propertyInfo, Func<object, object> conversion)
    {
        ConversionRules.Add(propertyInfo, new KeyValuePair<Type, Func<object, object>>(typeof(TType), conversion));
    }
}