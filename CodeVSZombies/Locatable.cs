using System;

namespace CodeVSZombies
{
    public class Locatable
    {
        public int x;
        public int y;
        public Locatable(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public override string ToString()
        {
            return this.x + " " + this.y;
        }

        public double DistanceTo(Locatable target)
        {
            return Math.Sqrt(Math.Pow((target.x - x), 2) + Math.Pow((target.y - y), 2));
        }
    }
}