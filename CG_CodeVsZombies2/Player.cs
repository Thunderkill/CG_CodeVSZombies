﻿namespace CG_CodeVsZombies2
{
    public class Player : ILocatable, IClonable<Player>
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Player(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Player Clone()
        {
            return new Player(X, Y);
        }
    }
}