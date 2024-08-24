using System.ComponentModel;

namespace Sadie.Shared.Helpers;

public static class EnumHelpers
{
    public static T GetEnumValueFromDescription<T>(string description)
    {
        var fieldInfos = typeof(T).GetFields();

        foreach (var fieldInfo in fieldInfos)
        {
            var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes.Length > 0 && attributes[0].Description == description)
            {
                return (T) Enum.Parse(typeof(T), fieldInfo.Name);
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