using System;
using CG_CodeVsZombies2.Utils;

namespace CG_CodeVsZombies2
{
    public class Simulator
    {
        public static Game Simulate(Game startingPosition, ILocatable playerTarget)
        {
            var newGame = startingPosition.Clone();

            // 1. First we move the zombies towards the closest human or player
            foreach (var zombie in newGame.Zombies.Values)
            {
                double closestDist = double.MaxValue;
                ILocatable closestEntity = default;
                foreach (var human in newGame.Humans.Values)
                {
                    var dist = DistanceUtils.FastDistanceTo(zombie, human);
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        closestEntity = human;
                    }
                }

                var playerDist = DistanceUtils.FastDistanceTo(newGame.Player, zombie);
                if (playerDist < closestDist)
                {
                    closestEntity = newGame.Player;
                }

                if (Math.Sqrt(closestDist) < 400)
                {
                    zombie.X = closestEntity!.X;
                    zombie.Y = closestEntity!.Y;
                }
                else
                {
                    EntityUtils.MoveTowards(zombie, closestEntity!, 400);
                }
            }

            // 2. Move the player
            if (DistanceUtils.DistanceTo(newGame.Player, playerTarget) < 1000)
            {
                newGame.Player.X = playerTarget.X;
                newGame.Player.Y = playerTarget.Y;
            }
            else
            {
                EntityUtils.MoveTowards(newGame.Player, playerTarget, 1000);
            }


            // 3. Kill the zombies that are close to the player
            var zombiesKilled = 0;
            foreach (var zombie in newGame.Zombies.Values)
            {
                var dist = DistanceUtils.DistanceTo(newGame.Player, zombie);
                if (dist > 2000) continue;

                newGame.Score += newGame.Humans.Count * newGame.Humans.Count * 10 *
                                 NumberUtils.Fibonacci(zombiesKilled + 2);
                newGame.Zombies.Remove(zombie.Id);
                zombiesKilled++;
            }

            // 4. Kill all the humans that are close to the zombies
            foreach (var zombie in newGame.Zombies.Values)
            {
                foreach (var human in newGame.Humans.Values)
                {
                    if (zombie.X == human.X && zombie.Y == human.Y)
                    {
                        newGame.Humans.Remove(human.Id);
                    }
                }
            }

            if (newGame.Zombies.Count == 0 || newGame.Humans.Count == 0)
            {
                newGame.GameEnded = true;
            }

            return newGame;
        }
    }
}