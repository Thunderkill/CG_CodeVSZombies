using System;

namespace CG_CodeVsZombies2.Utils
{
    public class AllowedDirections
    {
        public static Location[] Get = 
        {
            new (0, 0), // Stay
            new (1000, 0), // Right
            new (708,708), // Upper right
            new (0, 1000), // Up
            new (-708,708), // Upper left
            new (-1000, 0), // Left
            new (-708, -708), // Lower left
            new (0, -1000), // Down
            new (708, -708), // Lower right
            
            // HALF STEPS BELOW
            /*new (500, 0), // Right
            new (354,354), // Upper right
            new (0, 500), // Up
            new (-354,354), // Upper left
            new (-500, 0), // Left
            new (-354, -354), // Lower left
            new (0, -500), // Down
            new (354, -354) // Lower right*/
        };

        public static Location GetRandom()
        {
            return Get[Randomizer.Get.Next(0, Get.Length)];
        }
    }
}