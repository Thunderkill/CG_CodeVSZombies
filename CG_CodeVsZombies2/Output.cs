// Author: Thunderr
using System.Collections.Generic;
using System;
using System.Diagnostics;
namespace CG_CodeVsZombies2
{
public struct Game : IClonable<Game>
{
public Dictionary<int, Zombie> Zombies { get; set; }
public Dictionary<int, Human> Humans { get; set; }
public int Score { get; set; }
public bool GameEnded { get; set; }
public bool PlayerWon { get; set; }
public Player Player { get; set; }
public Game(Player player)
{
Zombies = new Dictionary<int, Zombie>();
Humans = new Dictionary<int, Human>();
Player = player;
Score = 0;
GameEnded = false;
PlayerWon = false;
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
newGame.Humans.Add(human.Key, human.Value);
}
newGame.Score = Score;
newGame.GameEnded = GameEnded;
newGame.PlayerWon = PlayerWon;
return newGame;
}
}
public struct Human : ILocatable, IIdentifiable
{
public int Id { get; set; }
public int X { get; set; }
public int Y { get; set; }
public Human(int id, int x, int y)
{
Id = id;
X = x;
Y = y;
}
}
public interface IClonable<T>
{
public T Clone();
}
public interface IIdentifiable
{
public int Id { get; set; }
}
public interface ILocatable
{
public int X { get; set; }
public int Y { get; set; }
}
public class Location : ILocatable
{
public int X { get; set; }
public int Y { get; set; }
public Location(int x, int y)
{
X = x;
Y = y;
}
public override string ToString()
{
return $"{X} {Y}";
}
}
public class Player : ILocatable, IClonable<Player>
{
public int X { get; set; }
public int Y { get; set; }
public Player(int x, int y)
{
X = x;
Y = y;
}
public Player Clone()
{
return new Player(X, Y);
}
}
internal class Program
{
public static void Main()
{
string[] inputs;
Player player = new Player(0, 0);
Game game = new Game(player);
bool initialized = false;
int maxSimulatedRounds = 100;
Simulation? previousBestSimulation = null;
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
if (watch.ElapsedMilliseconds > 98)
{
Console.Error.WriteLine("Managed to do {0} evolutions", evolution);
break;
}
var evolutionGame = game.Clone();
var moves = new List<Location>();
for (int round = 0; round < maxSimulatedRounds; round++)
{
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
if (closestDist < 160000)
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
if (DistanceUtils.FastDistanceTo(game.Player, playerTarget) < 1000000)
{
game.Player.X = playerTarget.X;
game.Player.Y = playerTarget.Y;
}
else
{
EntityUtils.MoveTowards(game.Player, playerTarget, 1000);
}
// 3. Kill the zombies that are close to the player
var zombiesKilled = 0;
foreach (var zombie in game.Zombies.Values)
{
var dist = DistanceUtils.FastDistanceTo(game.Player, zombie);
if (dist > 4000000) continue;
game.Score += game.Humans.Count * game.Humans.Count * 10 *
NumberUtils.Fibonacci(zombiesKilled + 2);
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
public class Zombie : ILocatable, IIdentifiable, IClonable<Zombie>
{
public int Id { get; set; }
public int X { get; set; }
public int Y { get; set; }
public int NextX { get; set; }
public int NextY { get; set; }
public Zombie(int id, int x, int y, int nextX, int nextY)
{
Id = id;
X = x;
Y = y;
NextX = nextX;
NextY = nextY;
}
public Zombie Clone()
{
return new Zombie(Id, X, Y, NextX, NextY);
}
}
public class AllowedDirections
{
/*public static Location[] Get = new[]
{
new Location(1000, 0),
new Location(-1000, 0),
new Location(0, 1000),
new Location(0, -1000)
};*/
public static Location[] Get = new[]
{
new Location(0, 0), // Stay
new Location(1000, 0), // Right
new Location((int)Math.Round(1000 * Math.Cos(Math.PI / 4)),
(int)Math.Round(1000 * Math.Sin(Math.PI / 4))), // Upper right
new Location(0, 1000), // Up
new Location((int)Math.Round(-1000 * Math.Cos(Math.PI / 4)),
(int)Math.Round(1000 * Math.Sin(Math.PI / 4))), // Upper left
new Location(-1000, 0), // Left
new Location((int)Math.Round(-1000 * Math.Cos(Math.PI / 4)),
(int)Math.Round(-1000 * Math.Sin(Math.PI / 4))), // Lower left
new Location(0, -1000), // Down
new Location((int)Math.Round(1000 * Math.Cos(Math.PI / 4)),
(int)Math.Round(-1000 * Math.Sin(Math.PI / 4))) // Lower right
};
public static Location GetRandom()
{
return Get[Random.Shared.Next(0, Get.Length)];
}
}
public static class DistanceUtils
{
public static double FastDistanceTo(ILocatable from, ILocatable to)
{
int deltaX = to.X - from.X;
int deltaY = to.Y - from.Y;
return deltaX * deltaX + deltaY * deltaY;
}
public static double DistanceTo(ILocatable from, ILocatable to)
{
return Math.Sqrt(FastDistanceTo(from, to));
}
}
public class EntityUtils
{
public static void MoveTowards(ILocatable from, ILocatable to, double units)
{
// Calculate the direction vector from 'from' to 'to'
double deltaX = to.X - from.X;
double deltaY = to.Y - from.Y;
// Calculate the length of the direction vector
double length = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
// Normalize the direction vector to get a unit vector
double unitDeltaX = deltaX / length;
double unitDeltaY = deltaY / length;
// Calculate the new coordinates
double newX = from.X + unitDeltaX * units;
double newY = from.Y + unitDeltaY * units;
// Set the new coordinates for 'from' (replace the below lines with your actual code)
from.X = (int)newX;
from.Y = (int)newY;
}
/*public static Location GetValidRandomLocation(ILocatable start, int maxRange)
{
var x = Random.Shared.Next(Math.Max(start.X - maxRange, 0), Math.Min(start.X + maxRange, 16000));
var y = Random.Shared.Next(Math.Max(start.Y - maxRange, 0), Math.Min(start.Y + maxRange, 9000));
return new Location(x, y);
}*/
public static Location GetValidRandomLocation(ILocatable start)
{
for (int i = 0; i < 10; i++)
{
var dir = AllowedDirections.GetRandom();
var x = start.X + dir.X;
var y = start.Y + dir.Y;
if (x < 0 || x > 16000 || y < 0 || y > 9000) continue;
return new Location(x, y);
}
return new Location(start.X, start.Y);
}
}
public static class NumberUtils
{
public static int Fibonacci(int n)
{
if (n <= 1) return n;
int fib = 1;
int prevFib = 1;
for (int i = 2; i < n; i++)
{
int temp = fib;
fib += prevFib;
prevFib = temp;
}
return fib;
}
}
}
