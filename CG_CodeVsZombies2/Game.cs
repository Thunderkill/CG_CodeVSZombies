﻿using System.Collections.Generic;

namespace CG_CodeVsZombies2
{
    public struct Game : IClonable<Game>
    {
        public Dictionary<int, Zombie> Zombies { get; set; }
        public Dictionary<int, Human> Humans { get; set; }

        public int Score { get; set; }
        public Player Player { get; set; }

        public Game(Player player)
        {
            Zombies = new Dictionary<int, Zombie>();
            Humans = new Dictionary<int, Human>();
            Player = player;
            Score = 0;
        }

        public Game Clone()
        {
            var newGame = new Game(Player.Clone());

            foreach (var zombie in Zombies)
            {
                newGame.Zombies.Add(zombie.Key, zombie.Value.Clone());
            }

            foreach (var human in Humans)
            {
                newGame.Humans.Add(human.Key, human.Value.Clone());
            }

            newGame.Score = Score;

            return newGame;
        }
    }
}