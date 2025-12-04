using System.Collections;
using System.Collections.ObjectModel;
using System.Reflection;
using Sadie.API;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Packets.Serialization
{
    public static class NetworkPacketWriterSerializer
    {
        private static readonly Dictionary<Type, Action<object, NetworkPacketWriter>> primitiveWriters =
            new()
            {
                { typeof(string), (v, w) => w.WriteString((string)v) },
                { typeof(int), (v, w) => w.WriteInteger((int)v) },
                { typeof(short), (v, w) => w.WriteShort((short)v) },
                { typeof(long), (v, w) => w.WriteLong((long)v) },
                { typeof(bool), (v, w) => w.WriteBool((bool)v) }
            };

        private static readonly Dictionary<Type, PropertyInfo[]> propertyCache = new();

        private static PropertyInfo[] GetCachedProperties(Type type)
        {
            if (propertyCache.TryGetValue(type, out var props))
            {
                return props;
            }

            props = type.GetProperties();
            propertyCache[type] = props;
            return props;
        }

        private static void InvokeOnConfigureRules(object packet)
        {
            var method = packet.GetType().GetMethod("OnConfigureRules");
            
            if (method == null)
            {
                return;
            }
            
            method.Invoke(packet, []);
        }

        private static bool InvokeOnSerializeIfExists(object packet, NetworkPacketWriter writer)
        {
            var method = packet.GetType().GetMethod("OnSerialize");
            
            if (method == null || method.GetBaseDefinition().DeclaringType == method.DeclaringType)
            {
                return false;
            }

            method.Invoke(packet, [writer]);
            return true;

        }

        private static short GetPacketIdentifierFromAttribute(object packetObject)
        {
            var attr = packetObject.GetType().GetCustomAttribute<PacketIdAttribute>();

            if (attr == null)
            {
                throw new InvalidOperationException(
                    $"Missing packet identifier attribute for packet type {packetObject.GetType()}"
                );
            }

            return attr.Id;
        }

        private static Dictionary<PropertyInfo, Action<INetworkPacketWriter>> GetRuleMap(object classObject, string name)
        {
            return (Dictionary<PropertyInfo, Action<INetworkPacketWriter>>)classObject
                .GetType()
                .BaseType?
                .GetProperty(name)?
                .GetValue(classObject);
        }

        private static Dictionary<PropertyInfo, Action<INetworkPacketWriter>> GetBeforeRuleMap(object obj)
        {
            return GetRuleMap(obj, "BeforeRulesSerialize");
        }

        private static Dictionary<PropertyInfo, Action<INetworkPacketWriter>> GetInsteadRuleMap(object obj)
        {
            return GetRuleMap(obj, "InsteadRulesSerialize");
        }

        private static Dictionary<PropertyInfo, Action<INetworkPacketWriter>> GetAfterRuleMap(object obj)
        {
            return GetRuleMap(obj, "AfterRulesSerialize");
        }

        private static Dictionary<PropertyInfo, KeyValuePair<Type, Func<object, object>>> GetConversionRules(object obj)
        {
            return (Dictionary<PropertyInfo, KeyValuePair<Type, Func<object, object>>>)obj
                .GetType()
                .BaseType?
                .GetProperty("ConversionRules")?
                .GetValue(obj);
        }

        private static bool TryWritePrimitive(PropertyInfo property, object packet, NetworkPacketWriter writer)
        {
            if (primitiveWriters.TryGetValue(property.PropertyType, out var action))
            {
                var value = property.GetValue(packet);
                action(value, writer);
                return true;
            }

            return false;
        }

        private static void WriteDictionary<TKey, TValue>(
            Dictionary<TKey, TValue> dict,
            NetworkPacketWriter writer,
            Action<TKey> keyWriter,
            Action<TValue> valueWriter
        )
        {
            writer.WriteInteger(dict.Count);

            foreach (var kv in dict)
            {
                keyWriter(kv.Key);
                valueWriter(kv.Value);
            }
        }

        private static void AddObjectToWriter(object packet, NetworkPacketWriter writer, bool needsAttribute = false)
        {
            var props =
                GetCachedProperties(packet.GetType())
                .Where(p =>
                {
                    if (needsAttribute == false)
                    {
                        return true;
                    }

                    return Attribute.IsDefined(p, typeof(PacketDataAttribute));
                });

            var conversionRules =
                GetConversionRules(packet);

            var beforeRules =
                GetBeforeRuleMap(packet);

            var insteadRules =
                GetInsteadRuleMap(packet);

            var afterRules =
                GetAfterRuleMap(packet);

            foreach (var property in props)
            {
                if (conversionRules != null && conversionRules.TryGetValue(property, out var conv))
                {
                    var raw = property.GetValue(packet);
                    var converted = conv.Value(raw);
                    WriteType(conv.Key, converted, writer);
                    continue;
                }

                if (insteadRules != null && insteadRules.TryGetValue(property, out var instead))
                {
                    instead(writer);
                    continue;
                }

                if (beforeRules != null && beforeRules.TryGetValue(property, out var before))
                {
                    before(writer);
                }

                WriteProperty(property, writer, packet);

                if (afterRules != null && afterRules.TryGetValue(property, out var after))
                {
                    after(writer);
                }
            }
        }

        public static INetworkPacketWriter Serialize(object packet)
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

        private static void WriteStringListPropertyToWriter(List<string> list, NetworkPacketWriter writer)
        {
            writer.WriteInteger(list.Count);

            foreach (var s in list)
            {
                writer.WriteString(s ?? "");
            }
        }

        private static void WriteArbitraryListPropertyToWriter(PropertyInfo property, NetworkPacketWriter writer, object packet)
        {
            var collection = (ICollection)property.GetValue(packet);

            writer.WriteInteger(collection.Count);

            foreach (var item in collection)
            {
                var props = GetCachedProperties(item.GetType());

                foreach (var p in props)
                {
                    WriteProperty(p, writer, item);
                }
            }
        }

        private static void WriteProperty(PropertyInfo property, NetworkPacketWriter writer, object packet)
        {
            if (TryWritePrimitive(property, packet, writer))
            {
                return;
            }

            var type = property.PropertyType;
            var value = property.GetValue(packet);

            if (type == typeof(List<string>))
            {
                WriteStringListPropertyToWriter((List<string>)value, writer);
                return;
            }

            if (type == typeof(Dictionary<int, string>))
            {
                WriteDictionary((Dictionary<int, string>)value, writer, writer.WriteInteger, v => writer.WriteString(v ?? ""));
                return;
            }

            if (type == typeof(Dictionary<long, string>))
            {
                WriteDictionary((Dictionary<long, string>)value, writer, writer.WriteLong, v => writer.WriteString(v ?? ""));
                return;
            }

            if (type == typeof(Dictionary<int, long>))
            {
                WriteDictionary((Dictionary<int, long>)value, writer, writer.WriteInteger, writer.WriteLong);
                return;
            }

            if (type == typeof(Dictionary<int, List<string>>))
            {
                var dict = (Dictionary<int, List<string>>)value;

                writer.WriteInteger(dict.Count);

                foreach (var kv in dict)
                {
                    writer.WriteInteger(kv.Key);

                    foreach (var s in kv.Value)
                    {
                        writer.WriteString(s);
                    }
                }

                return;
            }

            if (type == typeof(Dictionary<string, int>))
            {
                WriteDictionary((Dictionary<string, int>)value, writer, writer.WriteString, writer.WriteInteger);
                return;
            }

            if (type == typeof(Dictionary<string, string>))
            {
                WriteDictionary((Dictionary<string, string>)value, writer, writer.WriteString, writer.WriteString);
                return;
            }

            if (type.IsGenericType &&
                (type.GetGenericTypeDefinition() == typeof(List<>) ||
                 type.GetGenericTypeDefinition() == typeof(Collection<>)))
            {
                WriteArbitraryListPropertyToWriter(property, writer, packet);
                return;
            }

            if (type == typeof(Dictionary<PropertyInfo, Action<NetworkPacketWriter>>) ||
                type == typeof(Dictionary<PropertyInfo, KeyValuePair<Type, Func<object, object>>>))
            {
                WriteType(type, value, writer);
                return;
            }

            AddObjectToWriter(value, writer, true);
        }

        private static void WriteType(Type type, object value, NetworkPacketWriter writer)
        {
            if (primitiveWriters.TryGetValue(type, out var action))
            {
                action(value, writer);
            }
        }
    }
}
