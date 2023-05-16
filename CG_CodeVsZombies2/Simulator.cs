using System;
using CG_CodeVsZombies2.Utils;

namespace CG_CodeVsZombies2
{
    public class Simulator
    {
        public static void Simulate(ref Game game, ILocatable playerTarget)
        {
            // 1. First we move the zombies towards the closest human or player
            foreach (var zombie in game.Zombies.Values)
            {
                double closestDist = double.MaxValue;
                ILocatable closestEntity = default;
                foreach (var human in game.Humans.Values)
                {
                    var dist = DistanceUtils.FastDistanceTo(zombie, human);
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        closestEntity = human;
                    }
                }

                var playerDist = DistanceUtils.FastDistanceTo(game.Player, zombie);
                if (playerDist < closestDist)
                {
                    closestEntity = game.Player;
                }

                EntityUtils.MoveTowards(zombie, closestEntity!, 400);
            }

            // 2. Move the player
            EntityUtils.MoveTowards(game.Player, playerTarget, 1000);


            // 3. Kill the zombies that are close to the player
            var zombiesKilled = 0;
            foreach (var zombie in game.Zombies.Values)
            {
                var dist = DistanceUtils.FastDistanceTo(game.Player, zombie);
                if (dist > 4000000) continue;

                game.Score += NumberUtils.Score[game.Humans.Count] * NumberUtils.FibonacciNumbers[zombiesKilled];
                game.Zombies.Remove(zombie.Id);
                zombiesKilled++;
            }

            // 4. Kill all the humans that are close to the zombies
            foreach (var zombie in game.Zombies.Values)
            {
                foreach (var human in game.Humans.Values)
                {
                    if (zombie.X == human.X && zombie.Y == human.Y)
                    {
                        game.Humans.Remove(human.Id);
                        break;
                    }
                }
            }

            if (game.Humans.Count == 0)
            {
                game.GameEnded = true;
                game.PlayerWon = false;
            }

            if (game.Zombies.Count == 0)
            {
                game.GameEnded = true;
                game.PlayerWon = true;
            }
        }
    }
}