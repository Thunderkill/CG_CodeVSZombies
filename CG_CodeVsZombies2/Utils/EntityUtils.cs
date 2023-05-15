using System;

namespace CG_CodeVsZombies2.Utils
{
    public class EntityUtils
    {
        public static void MoveTowards(ILocatable from, ILocatable to, double units)
        {
            // Calculate the direction vector from 'from' to 'to'
            double deltaX = to.X - from.X;
            double deltaY = to.Y - from.Y;

            // Calculate the length of the direction vector
            double length = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

            // Normalize the direction vector to get a unit vector
            double unitDeltaX = deltaX / length;
            double unitDeltaY = deltaY / length;

            // Calculate the new coordinates
            double newX = from.X + unitDeltaX * units;
            double newY = from.Y + unitDeltaY * units;

            // Set the new coordinates for 'from' (replace the below lines with your actual code)
            from.X = (int)newX;
            from.Y = (int)newY;
        }
    }
}