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

            if (type == typeof(int))
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
            else
            {
                throw new Exception($"{type.FullName}");
            }
        }
    }
}