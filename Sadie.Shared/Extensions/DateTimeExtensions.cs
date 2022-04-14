namespace Sadie.Shared.Extensions;

public static class DateTimeExtensions
{
    public static long ToUnix(this DateTime dateTime)
    {
        return new DateTimeOffset(dateTime).ToUnixTimeSeconds();
    }
}