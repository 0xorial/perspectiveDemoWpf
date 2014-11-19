using System;

namespace perspectivePlayground
{
    public static class Extensions
    {
        public static bool EpsilonEquals(this float f, float other, float epsilon)
        {
            return Math.Abs(f - other) <= epsilon;
        }
    }
}