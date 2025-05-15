namespace Mailroom.Pages;

public static class TimeZoneUtil
{
    public static DateTime ConvertToUserTime(DateTime utc, string? timezoneId)
    {
        if (string.IsNullOrWhiteSpace(timezoneId))
            return utc;

        try
        {
            var tz = TimeZoneInfo.FindSystemTimeZoneById(timezoneId);
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utc, DateTimeKind.Utc), tz);
        }
        catch
        {
            return utc;
        }
    }

    public static DateTime ToUtc(DateTime local, string? timezoneId)
    {
        if (string.IsNullOrWhiteSpace(timezoneId))
            return local;

        var tz = TimeZoneInfo.FindSystemTimeZoneById(timezoneId);
        return TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(local, DateTimeKind.Unspecified), tz);
    }

    public static DateTime FromUtc(DateTime utc, string? timezoneId)
    {
        if (string.IsNullOrWhiteSpace(timezoneId))
            return utc;

        var tz = TimeZoneInfo.FindSystemTimeZoneById(timezoneId);
        return TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utc, DateTimeKind.Utc), tz);
    }
}