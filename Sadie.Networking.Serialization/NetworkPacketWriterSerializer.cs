using System.Collections;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Reflection;
using Sadie.API;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Attributes;

namespace Sadie.Networking.Serialization;

public class NetworkPacketWriterSerializer
{
    private readonly ConcurrentDictionary<Type, short> _writerIdCache = new();
    
    private void InvokeOnConfigureRules(object packet)
    {
        var configureRulesMethod = packet.GetType().GetMethod("OnConfigureRules");
        configureRulesMethod?.Invoke(packet, []);
    }
    
    private bool InvokeOnSerializeIfExists(object packet, NetworkPacketWriter writer)
    {
        var onSerialize = packet.GetType().GetMethod("OnSerialize");

        if (onSerialize == null)
        {
            return false;
        }
        
        if (onSerialize.GetBaseDefinition().DeclaringType == onSerialize.DeclaringType)
        {
            return false;
        }
        
        onSerialize.Invoke(packet, [writer]);
        return true;
    }

    private short GetPacketIdentifierFromAttribute(object packetObject)
    {
        if (_writerIdCache.ContainsKey(packetObject.GetType()))
        {
            return _writerIdCache[packetObject.GetType()];
        }
        
        var identifierAttribute = packetObject.GetType().GetCustomAttribute<PacketIdAttribute>();

        if (identifierAttribute == null)
        {
            throw new InvalidOperationException($"Missing packet identifier attribute for packet type {packetObject.GetType()}");
        }

        _writerIdCache[packetObject.GetType()] = identifierAttribute.Id;
        return identifierAttribute.Id;
    }
    
    private static Dictionary<PropertyInfo, Action<INetworkPacketWriter>> GetRuleMap(object classObject, string propertyName)
    {
        return (Dictionary<PropertyInfo, Action<INetworkPacketWriter>>) classObject
            .GetType()
            .BaseType?.GetProperty(propertyName)
            ?.GetValue(classObject)!;
    }

    private static Dictionary<PropertyInfo, Action<INetworkPacketWriter>> GetBeforeRuleMap(object classObject) => 
        GetRuleMap(classObject, "BeforeRulesSerialize");
    private static Dictionary<PropertyInfo, Action<INetworkPacketWriter>> GetInsteadRuleMap(object classObject) => 
        GetRuleMap(classObject, "InsteadRulesSerialize");

    private static Dictionary<PropertyInfo, Action<INetworkPacketWriter>> GetAfterRuleMap(object classObject) =>
        GetRuleMap(classObject, "AfterRulesSerialize");
    
    private static Dictionary<PropertyInfo, KeyValuePair<Type, Func<object, object>>> GetConversionRules(object classObject) => 
        (Dictionary<PropertyInfo, KeyValuePair<Type, Func<object, object>>>)
        classObject
            .GetType()
            .BaseType?.GetProperty("ConversionRules")
            ?.GetValue(classObject)!;
    
    private static void AddObjectToWriter(object packet, NetworkPacketWriter writer, bool needsAttribute = false)
    {
        var properties = packet
            .GetType()
            .GetProperties()
            .Where(p => !needsAttribute || Attribute.IsDefined(p, typeof(PacketDataAttribute)));
        
        var conversionRules = GetConversionRules(packet);        
        var beforeRuleMap = GetBeforeRuleMap(packet);
        var insteadRuleMap = GetInsteadRuleMap(packet);
        var afterRuleMap = GetAfterRuleMap(packet);

        foreach (var property in properties)
        {
            if (conversionRules != null && conversionRules.TryGetValue(property, out var rule))
            {
                WriteType(rule.Key, rule.Value.Invoke(property.GetValue(packet)!), writer);
                continue;
            }
            
            if (insteadRuleMap != null && 
                insteadRuleMap.TryGetValue(property, out var value))
            {
                value.Invoke(writer);
                continue;
            }

            if (beforeRuleMap != null && 
                beforeRuleMap.TryGetValue(property, out var beforeValue))
            {
                beforeValue.Invoke(writer);
            }
            
            WriteProperty(property, writer, packet);

            if (afterRuleMap != null && 
                afterRuleMap.TryGetValue(property, out var afterValue))
            {
                afterValue.Invoke(writer);
            }
        }
    }
    
    public NetworkPacketWriter Serialize(object packet)
    {
        var writer = new NetworkPacketWriter();
        writer.WriteShort(GetPacketIdentifierFromAttribute(packet));

        if (InvokeOnSerializeIfExists(packet, writer))
        {
            return writer;
        }
        
        InvokeOnConfigureRules(packet);
        AddObjectToWriter(packet, writer);

        return writer;
    }

    private static void WriteStringListPropertyToWriter(List<string?> list, NetworkPacketWriter writer)
    {
        writer.WriteInteger(list.Count);
            
        foreach (var item in list)
        {
            writer.WriteString(item ?? "");
        }
    }
    
    public static void WriteArbitraryListPropertyToWriter(PropertyInfo propertyInfo, NetworkPacketWriter writer, object packet)
    {
        var elements = (ICollection)propertyInfo.GetValue(packet)!;
        writer.WriteInteger(elements.Count);

        foreach (var element in elements)
        {
            var properties = element.GetType().GetProperties();
            
            foreach (var elementProperty in properties)
            {
                WriteProperty(elementProperty, writer, element);
            }
        }
    }

    private static void WriteProperty(PropertyInfo property, NetworkPacketWriter writer, object packet)
    {
        var type = property.PropertyType;
        
        if (type == typeof(string))
        {
            WriteType(typeof(string), property.GetValue(packet)!, writer);
        }
        else if (type == typeof(int))
        {
            WriteType(typeof(int), property.GetValue(packet)!, writer);
        }
        else if (type == typeof(long))
        {
            WriteType(typeof(long), property.GetValue(packet)!, writer);
        }
        else if (type == typeof(bool))
        {
            WriteType(typeof(bool), property.GetValue(packet)!, writer);
        }
        else if (type == typeof(List<string>))
        {
            var collection = (List<string?>)property.GetValue(packet)!;
            WriteStringListPropertyToWriter(collection, writer);
        }
        else if (type == typeof(Dictionary<int, string>))
        {
            var collection = (Dictionary<int, string?>)property.GetValue(packet)!;
            
            writer.WriteInteger(collection.Count);
            
            foreach (var (key, value) in collection)
            {
                writer.WriteInteger(key);
                writer.WriteString(value ?? "");
            }
        }
        else if (type == typeof(Dictionary<int, long>))
        {
            var collection = (Dictionary<int, long>)property.GetValue(packet)!;
            
            writer.WriteInteger(collection.Count);
            
            foreach (var (key, value) in collection)
            {
                writer.WriteInteger(key);
                writer.WriteLong(value);
            }
        }
        else if (type == typeof(Dictionary<int, List<string>>))
        {
            var collection = (Dictionary<int, List<string>>)property.GetValue(packet)!;
            
            writer.WriteInteger(collection.Count);
            
            foreach (var (key, value) in collection)
            {
                writer.WriteInteger(key);

                foreach (var inner in value)
                {
                    writer.WriteString(inner);
                }
            }
        }
        else if (type == typeof(Dictionary<string, int>))
        {
            var collection = (Dictionary<string, int>)property.GetValue(packet)!;
            
            writer.WriteInteger(collection.Count);
            
            foreach (var (key, value) in collection)
            {
                writer.WriteString(key);
                writer.WriteInteger(value);
            }
        }
        else if (type == typeof(Dictionary<string, string>))
        {
            var collection = (Dictionary<string, string>)property.GetValue(packet)!;
            
            writer.WriteInteger(collection.Count);
            
            foreach (var (key, value) in collection)
            {
                writer.WriteString(key);
                writer.WriteString(value);
            }
        }
        else if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(List<>) || type.GetGenericTypeDefinition() == typeof(Collection<>)))
        {
            WriteArbitraryListPropertyToWriter(property, writer, packet);
        }
        else if (type != typeof(Dictionary<PropertyInfo, Action<NetworkPacketWriter>>) && 
                 type != typeof(Dictionary<PropertyInfo, KeyValuePair<Type, Func<object, object>>>))
        {
            AddObjectToWriter(property.GetValue(packet)!, writer, true);
        }
    }

    private static void WriteType(Type type, object value, NetworkPacketWriter writer)
    {
        if (type == typeof(string))
        {
            writer.WriteString((value as string)!);
        }
        else if (type == typeof(int))
        {
            writer.WriteInteger((int)value);
        }
        else if (type == typeof(long))
        {
            writer.WriteLong((long)value);
        }
        else if (type == typeof(bool))
        {
            writer.WriteBool((bool) value);
        }
    }
}