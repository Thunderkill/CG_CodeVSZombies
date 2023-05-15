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

        /*public static Location GetValidRandomLocation(ILocatable start, int maxRange)
        {
            var x = Random.Shared.Next(Math.Max(start.X - maxRange, 0), Math.Min(start.X + maxRange, 16000));
            var y = Random.Shared.Next(Math.Max(start.Y - maxRange, 0), Math.Min(start.Y + maxRange, 9000));
            return new Location(x, y);
        }*/

        public static Location GetValidRandomLocation(ILocatable start)
        {
            for (int i = 0; i < 10; i++)
            {
                var dir = AllowedDirections.GetRandom();
                var x = start.X + dir.X;
                var y = start.Y + dir.Y;
                if (x < 0 || x > 16000 || y < 0 || y > 9000) continue;
                return new Location(x, y);
            }

            return new Location(start.X, start.Y);
        }
    }
}