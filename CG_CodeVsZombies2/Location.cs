using System;

namespace CG_CodeVsZombies2
{
    public struct Location : ILocatable
    {
        public short X { get; set; }
        public short Y { get; set; }

        public Location(short x, short y)
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