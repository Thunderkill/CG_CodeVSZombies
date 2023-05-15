using System.Collections.Generic;

namespace CG_CodeVsZombies2
{
    public struct Game
    {
        public Dictionary<byte, Location> Zombies { get; set; }
        public Dictionary<byte, Location> Humans { get; set; }

        public int Score { get; set; }

        public bool GameEnded { get; set; }

        public bool PlayerWon { get; set; }

        public Location PlayerLocation { get; set; }

        public Game(Location playerLocation)
        {
            Zombies = new Dictionary<byte, Location>();
            Humans = new Dictionary<byte, Location>();
            PlayerLocation = playerLocation;
            Score = 0;
            GameEnded = false;
            PlayerWon = false;
        }

        public Game(Location playerLocation, Dictionary<byte, Location> previousZombies,
            Dictionary<byte, Location> previousHumans)
        {
            Zombies = new Dictionary<byte, Location>(previousZombies);
            Humans = new Dictionary<byte, Location>(previousHumans);
            PlayerLocation = playerLocation;
            Score = 0;
            GameEnded = false;
            PlayerWon = false;
        }

        public Game Clone()
        {
            var newGame = new Game(PlayerLocation, Zombies, Humans);

            newGame.Score = Score;

            newGame.GameEnded = GameEnded;

            newGame.PlayerWon = PlayerWon;

            return newGame;
        }
    }
}