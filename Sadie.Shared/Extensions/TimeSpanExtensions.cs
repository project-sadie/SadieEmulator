namespace Sadie.Shared.Extensions;

public static class TimeSpanExtensions
{
    public static string ToTimeAgo(this TimeSpan timeSpan, bool @short = false)
    {
        // TODO: fix this lol
        
        var days = TimeSpan.FromSeconds(timeSpan.TotalSeconds).Days;
        var hours = TimeSpan.FromSeconds(timeSpan.TotalSeconds).Hours;
        var minutes = TimeSpan.FromSeconds(timeSpan.TotalSeconds).Minutes;
        var seconds = (int) timeSpan.TotalSeconds;
        
        if (timeSpan.TotalDays >= 1)
        {
            return $"{days} days, {hours} hours, {minutes} minutes and {seconds} seconds";
        }

        if (timeSpan.TotalHours >= 1)
        {
            return $"{hours} hours, {minutes} minutes and {seconds} seconds";
        }
        
        if (timeSpan.TotalMinutes >= 1)
        {
            return $"{minutes} minutes and {seconds} seconds";
        }
        
        return $"{seconds} seconds";
    }
}