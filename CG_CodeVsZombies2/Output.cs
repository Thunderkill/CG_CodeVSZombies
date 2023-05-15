// Author: Thunderr
using System.Collections.Generic;
using System;
using System.Diagnostics;
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
public interface ILocatable
{
public short X { get; set; }
public short Y { get; set; }
}
public struct Location : ILocatable
{
public short X { get; set; }
public short Y { get; set; }
public Location(short x, short y)
{
X = x;
Y = y;
}
public override string ToString()
{
return $"{X} {Y}";
}
}
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
new Location((short)Math.Round(1000 * Math.Cos(Math.PI / 4)),
(short)Math.Round(1000 * Math.Sin(Math.PI / 4))), // Upper right
new Location(0, 1000), // Up
new Location((short)Math.Round(-1000 * Math.Cos(Math.PI / 4)),
(short)Math.Round(1000 * Math.Sin(Math.PI / 4))), // Upper left
new Location(-1000, 0), // Left
new Location((short)Math.Round(-1000 * Math.Cos(Math.PI / 4)),
(short)Math.Round(-1000 * Math.Sin(Math.PI / 4))), // Lower left
new Location(0, -1000), // Down
new Location((short)Math.Round(1000 * Math.Cos(Math.PI / 4)),
(short)Math.Round(-1000 * Math.Sin(Math.PI / 4))) // Lower right
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
}
public class EntityUtils
{
public static Location MoveTowards(ILocatable from, ILocatable to, double units)
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
//from.X = (short)newX;
//from.Y = (short)newY;
return new Location((short)newX, (short)newY);
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
short x = (short)(start.X + dir.X);
short y = (short)(start.Y + dir.Y);
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
