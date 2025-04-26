using System.Reflection;
using Sadie.API.Networking.Packets;

namespace Sadie.Networking;

public static class EventSerializer
{
    public static void SetPropertiesForEventHandler(object handler, INetworkPacket packet)
    {
        var t = handler.GetType();
        var properties = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            var type = property.PropertyType;

            if (type == typeof(int) || type == typeof(long))
            {
                property.SetValue(handler, packet.ReadInt(), null);
            }
            else if (type == typeof(string))
            {
                property.SetValue(handler, packet.ReadString(), null);
            }
            else if (type == typeof(bool))
            {
                property.SetValue(handler, packet.ReadBool(), null);
            }
            else if (type == typeof(List<string>))
            {
                property.SetValue(handler, ReadStringList(packet), null);
            }
            else if (type == typeof(List<int>))
            {
                property.SetValue(handler, ReadIntegerList(packet), null);
            }
            else if (type == typeof(List<long>))
            {
                property.SetValue(handler, ReadLongList(packet), null);
            }
            else if (type == typeof(Dictionary<string, string>))
            {
                property.SetValue(handler, ReadAllStringDictionary(packet), null);
            }
            else
            {
                throw new Exception($"{type.FullName}");
            }
        }
    }

    private static Dictionary<string, string> ReadAllStringDictionary(INetworkPacketReader packet)
    {
        var temp = new Dictionary<string, string>();
        var amount = packet.ReadInt();

        for (var i = 0; i < amount / 2; i++)
        {
            temp[packet.ReadString()] = packet.ReadString();
        }

        return temp;
    }

    private static List<int> ReadIntegerList(INetworkPacketReader packet)
    {
        var tempList = new List<int>();
        var amount = packet.ReadInt();

        for (var i = 0; i < amount; i++)
        {
            tempList.Add(packet.ReadInt());
        }

        return tempList;
    }

    private static List<long> ReadLongList(INetworkPacketReader packet)
    {
        var tempList = new List<long>();
        var amount = packet.ReadInt();

        for (var i = 0; i < amount; i++)
        {
            tempList.Add(packet.ReadInt());
        }

        return tempList;
    }

    private static List<string> ReadStringList(INetworkPacketReader packet)
    {
        var tempList = new List<string>();

        for (var i = 0; i < packet.ReadInt(); i++)
        {
            tempList.Add(packet.ReadString());
        }

        return tempList;
    }
}