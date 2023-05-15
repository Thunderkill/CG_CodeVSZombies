using System;

namespace CG_CodeVsZombies2.Utils
{
    public class AllowedDirections
    {
        /*public static Location[] Get = new[]
        {
            new Location(1000, 0),
            new Location(-1000, 0),
            new Location(0, 1000),
            new Location(0, -1000)
        };*/

        public static Location[] Get = new[]
        {
            new Location(1000, 0), // Right
            new Location((int)Math.Round(1000 * Math.Cos(Math.PI / 4)),
                (int)Math.Round(1000 * Math.Sin(Math.PI / 4))), // Upper right
            new Location(0, 1000), // Up
            new Location((int)Math.Round(-1000 * Math.Cos(Math.PI / 4)),
                (int)Math.Round(1000 * Math.Sin(Math.PI / 4))), // Upper left
            new Location(-1000, 0), // Left
            new Location((int)Math.Round(-1000 * Math.Cos(Math.PI / 4)),
                (int)Math.Round(-1000 * Math.Sin(Math.PI / 4))), // Lower left
            new Location(0, -1000), // Down
            new Location((int)Math.Round(1000 * Math.Cos(Math.PI / 4)),
                (int)Math.Round(-1000 * Math.Sin(Math.PI / 4))) // Lower right
        };

        public static Location GetRandom()
        {
            return Get[Random.Shared.Next(0, Get.Length)];
        }
    }
}