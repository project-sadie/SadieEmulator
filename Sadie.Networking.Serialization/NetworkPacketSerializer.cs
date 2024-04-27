using System.Reflection;
using Sadie.Shared.Unsorted.Networking.Packets.Attributes;

namespace Sadie.Networking.Serialization;

public class NetworkPacketSerializer
{
    private static void InvokeOnConfigureRules(object packet)
    {
        var configureRulesMethod = packet.GetType().GetMethod("OnConfigureRules");
        configureRulesMethod?.Invoke(packet, []);
    }
    
    private static bool InvokeOnSerializeIfExists(object packet, NetworkPacketWriter writer)
    {
        var onSerialize = packet.GetType().GetMethod("OnSerialize");

        if (onSerialize == null)
        {
            return false;
        }
        
        onSerialize.Invoke(packet, [writer]);
        return true;
    }

    private static short GetPacketIdentifierFromAttribute(object packetObject)
    {
        var identifierAttribute = packetObject.GetType().GetCustomAttribute<PacketIdAttribute>();

        if (identifierAttribute == null)
        {
            throw new InvalidOperationException($"Missing packet identifier attribute for packet type {packetObject.GetType()}");
        }

        return identifierAttribute.Id;
    }
    
    private static Dictionary<PropertyInfo, Action<NetworkPacketWriter>> GetRuleMap(object classObject, string propertyName)
    {
        return (Dictionary<PropertyInfo, Action<NetworkPacketWriter>>) classObject
            .GetType()
            .BaseType?.GetProperty(propertyName)
            ?.GetValue(classObject)!;
    }

    private static Dictionary<PropertyInfo, Action<NetworkPacketWriter>> GetBeforeRuleMap(object classObject) => 
        GetRuleMap(classObject, "BeforeRulesSerialize");
    private static Dictionary<PropertyInfo, Action<NetworkPacketWriter>> GetInsteadRuleMap(object classObject) => 
        GetRuleMap(classObject, "InsteadRulesSerialize");

    private static Dictionary<PropertyInfo, Action<NetworkPacketWriter>> GetAfterRuleMap(object classObject) =>
        GetRuleMap(classObject, "AfterRulesSerialize");
    
    private static Dictionary<PropertyInfo, KeyValuePair<Type, Func<object, object>>> GetConversionRules(object classObject) => 
        (Dictionary<PropertyInfo, KeyValuePair<Type, Func<object, object>>>)
        classObject
            .GetType()
            .BaseType?.GetProperty("ConversionRules")
            ?.GetValue(classObject)!;
    
    private static void AddObjectToWriter(object packet, NetworkPacketWriter writer)
    {
        writer.WriteShort(GetPacketIdentifierFromAttribute(packet));
        
        var properties = packet.GetType().GetProperties()
            .Where(p => p.GetCustomAttributes(typeof(NotPacketDataAttribute), true).Length == 0);
        
        var conversionRules = GetConversionRules(packet);        
        var beforeRuleMap = GetBeforeRuleMap(packet);
        var insteadRuleMap = GetInsteadRuleMap(packet);
        var afterRuleMap = GetAfterRuleMap(packet);

        foreach (var property in properties)
        {
            if (conversionRules.TryGetValue(property, out var rule))
            {
                WriteType(rule.Key, rule.Value.Invoke(property.GetValue(packet)!), writer);
                continue;
            }
            
            if (insteadRuleMap.ContainsKey(property))
            {
                insteadRuleMap[property].Invoke(writer);
                continue;
            }

            if (beforeRuleMap.ContainsKey(property))
            {
                beforeRuleMap[property].Invoke(writer);
            }
            
            WriteProperty(property, writer, packet);

            if (afterRuleMap.ContainsKey(property))
            {
                afterRuleMap[property].Invoke(writer);
            }
        }
    }
    
    public static NetworkPacketWriter Serialize(object packet)
    {
        var writer = new NetworkPacketWriter();

        if (InvokeOnSerializeIfExists(packet, writer))
        {
            return writer;
        }
        
        InvokeOnConfigureRules(packet);
        AddObjectToWriter(packet, writer);

        return writer;
    }

    private static void WriteStringListPropertyToWriter(List<string> list, NetworkPacketWriter writer)
    {
        writer.WriteInteger(list.Count);
            
        foreach (var item in list)
        {
            writer.WriteString(item);
        }
    }

    private static void WriteArbitraryListPropertyToWriter(PropertyInfo propertyInfo, NetworkPacketWriter writer, object packet)
    {
        var collection = (List<object>)propertyInfo.GetValue(packet)!;
        writer.WriteInteger(collection.Count);

        var properties = collection
            .SelectMany(element =>
                element.GetType().GetProperties().Where(p => Attribute.IsDefined(p, typeof(PacketDataAttribute))));
        
        foreach (var elementProperty in properties)
        {
            WriteProperty(elementProperty, writer, packet);
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
        else if (type == typeof(bool))
        {
            WriteType(typeof(bool), property.GetValue(packet)!, writer);
        }
        else if (type == typeof(List<string>))
        {
            var collection = (List<string>)property.GetValue(packet)!;
            WriteStringListPropertyToWriter(collection, writer);
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
        else if (type == typeof(List<>))
        {
            WriteArbitraryListPropertyToWriter(property, writer, packet);
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
            writer.WriteInteger((int)value!);
        }
        else if (type == typeof(bool))
        {
            writer.WriteBool((bool) value!);
        }
    }
}