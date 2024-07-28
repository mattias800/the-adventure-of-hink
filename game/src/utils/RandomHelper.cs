using System;

namespace Theadventureofhink.utils;

public static class RandomHelper
{
    private static Random random = new Random();

    public static float RandfRange(float min, float max)
    {
        return (float)(random.NextDouble() * (max - min) + min);
    }
}