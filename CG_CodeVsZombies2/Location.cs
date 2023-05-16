﻿namespace CG_CodeVsZombies2
{
    public struct Location : ILocatable
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Location(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return $"{X} {Y}";
        }
    }
}