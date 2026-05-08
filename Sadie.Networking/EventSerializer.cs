using System.Reflection;
using Sadie.Networking.Packets;

namespace Sadie.Networking;

public static class EventSerializer
{
    public static void SetPropertiesForEventHandler(object handler, NetworkPacketReader packetReader)
    {
        var t = handler.GetType();
        var properties = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            var type = property.PropertyType;

            if (type == typeof(int) || type == typeof(long))
            {
                property.SetValue(handler, packetReader.ReadInt(), null);
            }
            else if (type == typeof(string))
            {
                property.SetValue(handler, packetReader.ReadString(), null);
            }
            else if (type == typeof(bool))
            {
                property.SetValue(handler, packetReader.ReadBool(), null);
            }
            else if (type == typeof(List<string>))
            {
                property.SetValue(handler, ReadStringList(packetReader), null);
            }
            else if (type == typeof(List<int>))
            {
                property.SetValue(handler, ReadIntegerList(packetReader), null);
            }
            else if (type == typeof(List<long>))
            {
                property.SetValue(handler, ReadLongList(packetReader), null);
            }
            else if (type == typeof(Dictionary<string, string>))
            {
                property.SetValue(handler, ReadAllStringDictionary(packetReader), null);
            }
            else
            {
                throw new Exception($"{type.FullName}");
            }
        }
    }

    private static Dictionary<string, string> ReadAllStringDictionary(NetworkPacketReader packetReader)
    {
        var temp = new Dictionary<string, string>();
        var amount = packetReader.ReadInt();

        for (var i = 0; i < amount / 2; i++)
        {
            temp[packetReader.ReadString()] = packetReader.ReadString();
        }

        return temp;
    }

    private static List<int> ReadIntegerList(NetworkPacketReader packetReader)
    {
        var tempList = new List<int>();
        var amount = packetReader.ReadInt();

        for (var i = 0; i < amount; i++)
        {
            tempList.Add(packetReader.ReadInt());
        }

        return tempList;
    }

    private static List<long> ReadLongList(NetworkPacketReader packetReader)
    {
        var tempList = new List<long>();
        var amount = packetReader.ReadInt();

        for (var i = 0; i < amount; i++)
        {
            tempList.Add(packetReader.ReadInt());
        }

        return tempList;
    }

    private static List<string> ReadStringList(NetworkPacketReader packetReader)
    {
        var tempList = new List<string>();

        for (var i = 0; i < packetReader.ReadInt(); i++)
        {
            tempList.Add(packetReader.ReadString());
        }

        return tempList;
    }
}