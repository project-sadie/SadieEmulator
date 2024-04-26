using System.Reflection;
using Sadie.Shared.Unsorted.Networking.Packets.Attributes;

namespace Sadie.Shared.Unsorted.Networking.Packets;

public class NetworkPacketSerializer
{
    private static void InvokeSerializeRuleInitialization(object packet)
    {
        var serializeOverrideMethod = packet.GetType().GetMethod("OnSerialize");
        serializeOverrideMethod?.Invoke(packet, []);
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
    
    private static void AddObjectToWriter(object packet, NetworkPacketWriter writer)
    {
        writer.WriteShort(GetPacketIdentifierFromAttribute(packet));
        
        var properties = packet.GetType().GetProperties();
        var beforeRuleMap = GetBeforeRuleMap(packet);
        var insteadRuleMap = GetInsteadRuleMap(packet);
        var afterRuleMap = GetAfterRuleMap(packet);

        foreach (var property in properties)
        {
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
        InvokeSerializeRuleInitialization(packet);
        
        var writer = new NetworkPacketWriter();
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
            writer.WriteString((property.GetValue(packet) as string)!);
        }
        else if (type == typeof(int))
        {
            writer.WriteInteger((int)property.GetValue(packet)!);
        }
        else if (type == typeof(bool))
        {
            writer.WriteBool((bool) property.GetValue(packet)!);
        }
        else if (type == typeof(List<string>))
        {
            var collection = (List<string>)property.GetValue(packet)!;
            WriteStringListPropertyToWriter(collection, writer);
        }
        else if (type == typeof(List<>))
        {
            WriteArbitraryListPropertyToWriter(property, writer, packet);
        }
    }
}