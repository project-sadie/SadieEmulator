namespace Sadie.Shared.Extensions;

public static class StringExtensions
{
    public static string Truncate(this string s, int maxLength)
    {
        return s[..maxLength];
    }
}