using System;

namespace CG_CodeVsZombies2.Utils
{
    public class EntityUtils
    {
        public static bool MoveTowards(ILocatable from, ILocatable to, float units)
        {
            // Calculate the direction vector from 'from' to 'to'
            float deltaX = to.X - from.X;
            float deltaY = to.Y - from.Y;

            // Calculate the length of the direction vector

            float lengthSq = deltaX * deltaX + deltaY * deltaY;
            float length = MathF.Sqrt(lengthSq);

            if (length < units)
            {
                from.X = to.X;
                from.Y = to.Y;
                return true;
            }

            // Normalize the direction vector to get a unit vector
            float unitDeltaX = deltaX / length;
            float unitDeltaY = deltaY / length;

            // Calculate the new coordinates
            float newX = from.X + unitDeltaX * units;
            float newY = from.Y + unitDeltaY * units;

            // Set the new coordinates for 'from' (replace the below lines with your actual code)
            from.X = (int)newX;
            from.Y = (int)newY;
            return false;
        }

        /*public static Location GetValidRandomLocation(ILocatable start, int maxRange)
        {
            var x = Randomizer.Get.Next(Math.Max(start.X - maxRange, 0), Math.Min(start.X + maxRange, 16000));
            var y = Randomizer.Get.Next(Math.Max(start.Y - maxRange, 0), Math.Min(start.Y + maxRange, 9000));
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