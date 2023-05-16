using System;

namespace CG_CodeVsZombies2.Utils
{
    public static class DistanceUtils
    {
        public static double FastDistanceTo(ILocatable from, ILocatable to)
        {
            int deltaX = to.X - from.X;
            int deltaY = to.Y - from.Y;

            return deltaX * deltaX + deltaY * deltaY;
        }
    }
}