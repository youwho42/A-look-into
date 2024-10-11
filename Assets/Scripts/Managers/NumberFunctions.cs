using System;

public static class NumberFunctions
{

    public static float RemapNumber(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    public static string GetTimeAsString(int ticks)
    {
        TimeSpan ts = TimeSpan.FromMinutes(ticks);

        return $"{ts.Hours}:{ts.Minutes:D2}";
    }
}
