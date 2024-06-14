using System.Reflection;
using Sadie.Networking.Packets;

namespace Sadie.Networking;

public class EventSerializer
{
    public static void SetEventHandlerProperties(object handler, INetworkPacket packet)
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
            else
            {
                throw new Exception($"{type.FullName}");
            }
        }
    }

    private static List<int> ReadIntegerList(INetworkPacket packet)
    {
        var tempList = new List<int>();
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