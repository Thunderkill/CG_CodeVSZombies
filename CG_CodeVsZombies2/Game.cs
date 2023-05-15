using System.Collections.Generic;

namespace CG_CodeVsZombies2
{
    public struct Game
    {
        public Dictionary<int, Zombie> Zombies { get; set; }
        public Dictionary<int, Human> Humans { get; set; }
        public Player Player { get; set; }

        public Game(Player player)
        {
            Zombies = new Dictionary<int, Zombie>();
            Humans = new Dictionary<int, Human>();
            Player = player;
        }
    }
}