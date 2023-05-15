using System;
using System.Collections.Generic;
using System.Diagnostics;
using CG_CodeVsZombies2.Utils;

namespace CG_CodeVsZombies2
{
    internal class Program
    {
        public static void Main()
        {
            string[] inputs;

            Game game = new Game(new Location(0, 0));
            bool initialized = false;
            int maxSimulatedRounds = 100;
            Simulation? previousBestSimulation = null;

            // game loop
            while (true)
            {
                if (!initialized)
                {
                    inputs = Console.ReadLine().Split(' ');
                    short x = short.Parse(inputs[0]);
                    short y = short.Parse(inputs[1]);
                    game = new Game(new Location(x, y));

                    int humanCount = int.Parse(Console.ReadLine());

                    for (int i = 0; i < humanCount; i++)
                    {
                        inputs = Console.ReadLine().Split(' ');
                        byte humanId = byte.Parse(inputs[0]);
                        short humanX = short.Parse(inputs[1]);
                        short humanY = short.Parse(inputs[2]);
                        var newHuman = new Location(humanX, humanY);
                        game.Humans.Add(humanId, newHuman);
                    }

                    int zombieCount = int.Parse(Console.ReadLine());
                    for (int i = 0; i < zombieCount; i++)
                    {
                        inputs = Console.ReadLine().Split(' ');
                        byte zombieId = byte.Parse(inputs[0]);
                        short zombieX = short.Parse(inputs[1]);
                        short zombieY = short.Parse(inputs[2]);
                        var newZombie = new Location(zombieX, zombieY);
                        game.Zombies.Add(zombieId, newZombie);
                    }

                    initialized = true;
                }
                else
                {
                    // This is a dummy reader that is only used after reading initial positions, cause we can fully simulate the rest
                    Console.ReadLine();
                    int humanCount = int.Parse(Console.ReadLine());
                    for (int i = 0; i < humanCount; i++)
                    {
                        Console.ReadLine();
                    }

                    int zombieCount = int.Parse(Console.ReadLine());
                    for (int i = 0; i < zombieCount; i++)
                    {
                        Console.ReadLine();
                    }
                }

                var watch = Stopwatch.StartNew();


                // DO ACTIONS HERE
                var bestSimulationScore = int.MinValue;
                Simulation bestSimulation = default;

                for (int evolution = 0; evolution < 100000; evolution++)
                {
                    if (watch.ElapsedMilliseconds > 96)
                    {
                        Console.Error.WriteLine("Managed to do {0} evolutions", evolution);
                        break;
                    }

                    Game evolutionGame = game.Clone();
                    var moves = new List<Location>();

                    for (int round = 0; round < maxSimulatedRounds; round++)
                    {
                        if (watch.ElapsedMilliseconds > 98)
                        {
                            Console.Error.WriteLine("Had to break mid evolution");
                            break;
                        }

                        var newLocation = EntityUtils.GetValidRandomLocation(evolutionGame.PlayerLocation);
                        Simulator.Simulate(ref evolutionGame, newLocation);
                        moves.Add(newLocation);
                        if (evolutionGame.GameEnded)
                        {
                            break;
                        }
                    }

                    if (evolutionGame.PlayerWon &&
                        evolutionGame.Score > bestSimulationScore)
                    {
                        bestSimulationScore = evolutionGame.Score;
                        bestSimulation = new Simulation(evolutionGame, moves);
                    }
                }

                if (previousBestSimulation.HasValue && previousBestSimulation.Value.Game.PlayerWon &&
                    previousBestSimulation.Value.Game.Score > bestSimulationScore)
                {
                    previousBestSimulation.Value.Moves.RemoveAt(0);
                    bestSimulation = previousBestSimulation.Value;
                }
                else
                {
                    previousBestSimulation = bestSimulation;
                }

                if (bestSimulationScore == int.MinValue)
                {
                    Console.Error.WriteLine("We did not find a solution");
                    if (game.Humans.Count == 3)
                    {
                        Console.Error.WriteLine("Just going towards #1");
                        Simulator.Simulate(ref game, game.Humans[1]);
                        Console.WriteLine(game.Humans[1].ToString());
                        return;
                    }

                    Double closestHumanDist = Double.MaxValue;
                    Location? closestHuman = null;
                    foreach (var human in game.Humans)
                    {
                        var dist = DistanceUtils.FastDistanceTo(human.Value, game.PlayerLocation);
                        if (dist > closestHumanDist) continue;
                        closestHumanDist = dist;
                        closestHuman = human.Value;
                    }

                    Simulator.Simulate(ref game, closestHuman!.Value);
                    Console.WriteLine(closestHuman!.Value.ToString());
                    return;
                }

                Console.Error.WriteLine(
                    "Best simulation has a ending score of {0} after {1} moves. There will be {2} humans left",
                    bestSimulation.Game.Score, bestSimulation.Moves.Count, bestSimulation.Game.Humans.Count);

                var target = bestSimulation.Moves[0];

                Simulator.Simulate(ref game, target);
                /*Console.Error.WriteLine(
                    $"Next round we should have {simulation.Score} score. {simulation.Humans.Count} humans alive");*/

                /*if (simulation.GameEnded)
                {
                    Console.Error.WriteLine("Game ended in the next round");
                }*/


                /*Console.Error.WriteLine("Player is at {0}, {1}", player.X, player.Y);

                foreach (var human in game.Humans)
                {
                    Console.Error.WriteLine(
                        $"Human {human.Key} is at {human.Value.X}, {human.Value.Y} and is {(human.Value.Alive ? "alive" : "dead")}");
                }

                foreach (var zombie in game.Zombies)
                {
                    Console.Error.WriteLine(
                        $"Zombie {zombie.Key} is at {zombie.Value.X}, {zombie.Value.Y}");
                }*/

                Console.WriteLine(target.ToString()); // Your destination coordinates
            }
        }
    }
}