using System.Collections.Generic;

namespace CG_CodeVsZombies2
{
    public struct Simulation
    {
        public Game Game { get; set; }
        public List<Location> Moves { get; set; }

        public Simulation(Game game, List<Location> moves)
        {
            Game = game;
            Moves = moves;
        }
    }
}