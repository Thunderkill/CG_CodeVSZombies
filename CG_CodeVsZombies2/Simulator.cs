using System;
using System.Collections.Generic;
using System.Linq;
using CG_CodeVsZombies2.Utils;

namespace CG_CodeVsZombies2
{
    public class Simulator
    {
        public static void Simulate(ref Game game, ILocatable playerTarget)
        {
            // 1. First we move the zombies towards the closest human or player
            var humansToBeKilled = new Dictionary<int, int>();
            foreach (var zombie in game.Zombies.Values)
            {
                double closestDist = double.MaxValue;
                ILocatable closestEntity = default;
                int? closestHumanId = null;
                foreach (var human in game.Humans.Values)
                {
                    var dist = DistanceUtils.FastDistanceTo(zombie, human);
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        closestEntity = human;
                        closestHumanId = human.Id;
                    }
                }

                var playerDist = DistanceUtils.FastDistanceTo(game.Player, zombie);
                if (playerDist < closestDist)
                {
                    closestEntity = game.Player;
                    closestHumanId = null;
                }

                if (EntityUtils.MoveTowards(zombie, closestEntity!, 400))
                {
                    if (closestHumanId.HasValue)
                    {
                        humansToBeKilled.Add(zombie.Id, closestHumanId!.Value);
                    }
                }
            }

            // 2. Move the player
            EntityUtils.MoveTowards(game.Player, playerTarget, 1000);


            // 3. Kill the zombies that are close to the player
            var zombiesKilled = 0;
            foreach (var zombie in game.Zombies.Values)
            {
                var dist = DistanceUtils.FastDistanceTo(game.Player, zombie);
                if (dist > 4000000) continue;

                if (humansToBeKilled.ContainsKey(zombie.Id))
                {
                    humansToBeKilled.Remove(zombie.Id);
                }

                game.Score += NumberUtils.Score[game.Humans.Count] * NumberUtils.FibonacciNumbers[zombiesKilled];
                game.Zombies.Remove(zombie.Id);
                zombiesKilled++;
            }

            // 4. Kill all the humans that are close to the zombies
            foreach (var humanId in humansToBeKilled.Values)
            {
                game.Humans.Remove(humanId);
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