namespace Theadventureofhink.utils;

public static class DurationFormatter
{
    public static string FormatSecondsAsPlayTime(int seconds)
    {
        var allMinutes = seconds / 60;
        if (allMinutes < 60)
        {
            return FormatMinutes(allMinutes);
        }

        var hours = allMinutes / 60;
        var minutes = allMinutes % 60;

        return $"{FormatHours(hours)} {FormatMinutes(minutes)}";
    }

    private static string FormatHours(int hours)
    {
        if (hours == 0)
        {
            return "";
        }
        else if (hours == 1)
        {
            return "1h";
        }
        else
        {
            return $"{hours}h";
        }
    }

    private static string FormatMinutes(int minutes)
    {
        if (minutes == 0)
        {
            return "0mins";
        }
        else if (minutes == 1)
        {
            return "1min";
        }
        else
        {
            return $"{minutes}mins";
        }
    }
}