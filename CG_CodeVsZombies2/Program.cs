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
            Stopwatch watch = new Stopwatch();
            string[] inputs;

            Player player = new Player(0, 0);
            Game game = new Game(player);
            bool initialized = false;
            int maxSimulatedRounds = 100;
            Simulation? previousBestSimulation = null;
            
            Console.Error.WriteLine("1. Elapsed: " + watch.ElapsedMilliseconds + "ms");

            // game loop
            while (true)
            {
                if (!initialized)
                {
                    inputs = Console.ReadLine().Split(' ');
                    int x = int.Parse(inputs[0]);
                    int y = int.Parse(inputs[1]);
                    player.X = x;
                    player.Y = y;

                    int humanCount = int.Parse(Console.ReadLine());

                    for (int i = 0; i < humanCount; i++)
                    {
                        inputs = Console.ReadLine().Split(' ');
                        int humanId = int.Parse(inputs[0]);
                        int humanX = int.Parse(inputs[1]);
                        int humanY = int.Parse(inputs[2]);
                        var newHuman = new Human(humanId, humanX, humanY);
                        game.Humans.Add(humanId, newHuman);
                    }

                    int zombieCount = int.Parse(Console.ReadLine());
                    for (int i = 0; i < zombieCount; i++)
                    {
                        inputs = Console.ReadLine().Split(' ');
                        int zombieId = int.Parse(inputs[0]);
                        int zombieX = int.Parse(inputs[1]);
                        int zombieY = int.Parse(inputs[2]);
                        int zombieXNext = int.Parse(inputs[3]);
                        int zombieYNext = int.Parse(inputs[4]);
                        var newZombie = new Zombie(zombieId, zombieX, zombieY, zombieXNext, zombieYNext);
                        game.Zombies.Add(zombieId, newZombie);
                    }

                    Console.Error.WriteLine("First init took: " + watch.ElapsedMilliseconds + "ms");
                    initialized = true;
                }
                else
                {
                    watch.Restart();
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

                    Console.Error.WriteLine("Reading inputs took: " + watch.ElapsedMilliseconds + "ms");
                }

                watch.Restart();

                // DO ACTIONS HERE
                var bestSimulationScore = int.MinValue;
                Simulation bestSimulation = default;

                for (int evolution = 0; evolution < 100000; evolution++)
                {
                    if (watch.ElapsedMilliseconds > 98)
                    {
                        Console.Error.WriteLine("Managed to do {0} evolutions", evolution);
                        Console.Error.WriteLine("EVOLUTIONS: {0}", evolution);
                        break;
                    }

                    var evolutionGame = game.Clone();
                    var moves = new List<Location>();

                    for (int round = 0; round < maxSimulatedRounds; round++)
                    {
                        if (watch.ElapsedMilliseconds > 98)
                        {
                            break;
                        }

                        var newLocation = EntityUtils.GetValidRandomLocation(evolutionGame.Player);
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
                    float closestHumanDist = float.MaxValue;
                    Human? closestHuman = null;
                    foreach (var human in game.Humans)
                    {
                        var dist = DistanceUtils.FastDistanceTo(human.Value, game.Player);
                        if (dist > closestHumanDist) continue;
                        closestHumanDist = dist;
                        closestHuman = human.Value;
                    }

                    Simulator.Simulate(ref game, closestHuman!.Value);
                    Console.WriteLine(closestHuman!.Value.ToString());
                    Console.Error.WriteLine(watch.ElapsedMilliseconds.ToString());
                    continue;
                }

                Console.Error.WriteLine(
                    "Best simulation has a ending score of {0} after {1} moves. There will be {2} humans left",
                    bestSimulation.Game.Score, bestSimulation.Moves.Count, bestSimulation.Game.Humans.Count);

                var target = bestSimulation.Moves[0];

                Simulator.Simulate(ref game, target);

                Console.WriteLine(target.ToString()); // Your destination coordinates
                Console.Error.WriteLine(watch.ElapsedMilliseconds.ToString());
            }
        }
    }
}