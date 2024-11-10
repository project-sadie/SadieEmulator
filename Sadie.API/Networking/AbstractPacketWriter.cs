using System.Reflection;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable CollectionNeverQueried.Local
// ReSharper disable CollectionNeverQueried.Global

namespace Sadie.API.Networking;

public abstract class AbstractPacketWriter
{
    public Dictionary<PropertyInfo, Action<INetworkPacketWriter>> InsteadRulesSerialize { get; } = new();
    public Dictionary<PropertyInfo, Action<INetworkPacketWriter>> AfterRulesSerialize { get; } = new();
    public Dictionary<PropertyInfo, KeyValuePair<Type, Func<object, object>>> ConversionRules { get; } = new();

    public virtual void OnConfigureRules()
    {
    }

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