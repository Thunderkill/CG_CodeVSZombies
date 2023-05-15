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

            Player player = new Player(0, 0);
            Game game = new Game(player);
            bool initialized = false;
            int maxSimulatedRounds = 20;

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

                    foreach (var human in game.Humans)
                    {
                        human.Value.Alive = false;
                    }

                    for (int i = 0; i < humanCount; i++)
                    {
                        inputs = Console.ReadLine().Split(' ');
                        int humanId = int.Parse(inputs[0]);
                        int humanX = int.Parse(inputs[1]);
                        int humanY = int.Parse(inputs[2]);
                        if (game.Humans.TryGetValue(humanId, out var human))
                        {
                            human.X = humanX;
                            human.Y = humanY;
                            human.Alive = true;
                        }
                        else
                        {
                            var newHuman = new Human(humanId, humanX, humanY);
                            game.Humans.Add(humanId, newHuman);
                        }
                    }

                    foreach (var human in game.Humans)
                    {
                        if (!human.Value.Alive)
                        {
                            game.Humans.Remove(human.Key);
                        }
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
                        if (game.Zombies.TryGetValue(zombieId, out var zombie))
                        {
                            zombie.X = zombieX;
                            zombie.Y = zombieY;
                        }
                        else
                        {
                            var newZombie = new Zombie(zombieId, zombieX, zombieY, zombieXNext, zombieYNext);
                            game.Zombies.Add(zombieId, newZombie);
                        }
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

                var simulations = new List<Simulation>();
                for (int evolution = 0; evolution < 10000; evolution++)
                {
                    if (watch.ElapsedMilliseconds > 90)
                    {
                        Console.Error.WriteLine("Managed to do {0} evolutions", evolution);
                        break;
                    }

                    var evolutionGame = game.Clone();
                    var moves = new List<Location>();
                    for (int round = 0; round < maxSimulatedRounds; round++)
                    {
                        var newLocation = EntityUtils.GetValidRandomLocation(evolutionGame.Player, 1000);
                        evolutionGame = Simulator.Simulate(evolutionGame, newLocation);
                        moves.Add(newLocation);
                    }

                    simulations.Add(new Simulation(evolutionGame, moves));
                }

                var bestSimulationScore = int.MinValue;
                Simulation bestSimulation = default;

                foreach (var sim in simulations)
                {
                    if (sim.Game.Score > bestSimulationScore)
                    {
                        bestSimulationScore = sim.Game.Score;
                        bestSimulation = sim;
                    }
                }

                Console.Error.WriteLine("Best simulation has a ending score of {0} after {1} moves",
                    bestSimulationScore, maxSimulatedRounds);

                var target = bestSimulation.Moves[0];

                var simulation = Simulator.Simulate(game, target);
                Console.Error.WriteLine(
                    $"Next round we should have {simulation.Score} score. {simulation.Humans.Count} humans alive");

                if (simulation.GameEnded)
                {
                    Console.Error.WriteLine("Game ended in the next round");
                }

                game = simulation;


                Console.Error.WriteLine("Player is at {0}, {1}", player.X, player.Y);

                foreach (var human in game.Humans)
                {
                    Console.Error.WriteLine(
                        $"Human {human.Key} is at {human.Value.X}, {human.Value.Y} and is {(human.Value.Alive ? "alive" : "dead")}");
                }

                foreach (var zombie in game.Zombies)
                {
                    Console.Error.WriteLine(
                        $"Zombie {zombie.Key} is at {zombie.Value.X}, {zombie.Value.Y}");
                }

                Console.WriteLine(target.ToString()); // Your destination coordinates
            }
        }
    }
}