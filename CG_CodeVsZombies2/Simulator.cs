using System;
using CG_CodeVsZombies2.Utils;

namespace CG_CodeVsZombies2
{
    public class Simulator
    {
        public static void Simulate(ref Game game, Location playerTarget)
        {
            // 1. First we move the zombies towards the closest human or player
            foreach (var pair in game.Zombies)
            {
                var zombie = pair.Value;
                double closestDist = double.MaxValue;
                Location closestEntity = default;
                foreach (var human in game.Humans.Values)
                {
                    var dist = DistanceUtils.FastDistanceTo(zombie, human);
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        closestEntity = human;
                    }
                }

                var playerDist = DistanceUtils.FastDistanceTo(game.PlayerLocation, zombie);
                if (playerDist < closestDist)
                {
                    closestEntity = game.PlayerLocation;
                }

                Location newLocation;
                if (closestDist < 160000)
                {
                    newLocation = closestEntity;
                }
                else
                {
                    newLocation = EntityUtils.MoveTowards(zombie, closestEntity, 400);
                }

                game.Zombies[pair.Key] = newLocation;
            }

            // 2. Move the player
            if (DistanceUtils.FastDistanceTo(game.PlayerLocation, playerTarget) < 1000000)
            {
                game.PlayerLocation = playerTarget;
            }
            else
            {
                var newLocation = EntityUtils.MoveTowards(game.PlayerLocation, playerTarget, 1000);
                game.PlayerLocation = newLocation;
            }


            // 3. Kill the zombies that are close to the player
            var zombiesKilled = 0;
            foreach (var zombiePair in game.Zombies)
            {
                var dist = DistanceUtils.FastDistanceTo(game.PlayerLocation, zombiePair.Value);
                if (dist > 4000000) continue;

                game.Score += game.Humans.Count * game.Humans.Count * 10 *
                              NumberUtils.Fibonacci(zombiesKilled + 2);
                game.Zombies.Remove(zombiePair.Key);
                zombiesKilled++;
            }

            // 4. Kill all the humans that are close to the zombies
            foreach (var zombie in game.Zombies.Values)
            {
                foreach (var humanPair in game.Humans)
                {
                    if (zombie.X == humanPair.Value.X && zombie.Y == humanPair.Value.Y)
                    {
                        game.Humans.Remove(humanPair.Key);
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