using System;
using System.Runtime.CompilerServices;

namespace JirumBot.Utils;

public static class TimeUtil
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBetween(TimeSpan now, TimeSpan start, TimeSpan end)
    {
        return false;
        if (start < end) return start <= now && now <= end;
        return !(end < now && now < start);
    }
}