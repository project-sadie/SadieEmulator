using System.ComponentModel;
using System.Reflection;

namespace Sadie.Shared.Helpers;

public class EnumHelpers
{
    public static T GetEnumValueFromDescription<T>(string description)
    {
        var fis = typeof(T).GetFields();

        foreach (var fi in fis)
        {
            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes.Length > 0 && attributes[0].Description == description)
            {
                return (T)Enum.Parse(typeof(T), fi.Name);
            }
        }

        throw new Exception($"Failed to resolve enum from description '{description}'");
    }
    
    public static string GetEnumDescription(Enum value)
    {
        return value.GetType()
            .GetField(value.ToString())
            ?.GetCustomAttributes(typeof(DescriptionAttribute), false)
            .SingleOrDefault() is not DescriptionAttribute attribute ? value.ToString() : attribute.Description;
    }
}