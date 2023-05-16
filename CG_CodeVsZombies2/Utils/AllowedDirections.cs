using System;

namespace CG_CodeVsZombies2.Utils
{
    public class AllowedDirections
    {
        public static Location[] Get = 
        {
            new (0, 0), // Stay
            new (1000, 0), // Right
            new (707,707), // Upper right
            new (0, 1000), // Up
            new (-707,707), // Upper left
            new (-1000, 0), // Left
            new (-707, -707), // Lower left
            new (0, -1000), // Down
            new (707, -707) // Lower right
        };

        public static Location GetRandom()
        {
            return Get[Randomizer.Get.Next(0, Get.Length)];
        }
    }
}