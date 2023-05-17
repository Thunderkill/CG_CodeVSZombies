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
public override string ToString()
{
return $"{X} {Y}";
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
public struct Location : ILocatable
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
Stopwatch watch = new Stopwatch();
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
public static class Randomizer
{
public static Random Get = new (1);
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
public static Location[] Get =
{
new (0, 0), // Stay
new (1000, 0), // Right
new (708,708), // Upper right
new (0, 1000), // Up
new (-708,708), // Upper left
new (-1000, 0), // Left
new (-708, -708), // Lower left
new (0, -1000), // Down
new (708, -708) // Lower right
};
public static Location GetRandom()
{
return Get[Randomizer.Get.Next(0, Get.Length)];
}
}
public static class DistanceUtils
{
public static float FastDistanceTo(ILocatable from, ILocatable to)
{
int deltaX = to.X - from.X;
int deltaY = to.Y - from.Y;
return deltaX * deltaX + deltaY * deltaY;
}
}
public class EntityUtils
{
public static void MoveTowards(ILocatable from, ILocatable to, float units)
{
// Calculate the direction vector from 'from' to 'to'
float deltaX = to.X - from.X;
float deltaY = to.Y - from.Y;
// Calculate the length of the direction vector
float lengthSq = deltaX * deltaX + deltaY * deltaY;
float length = MathF.Sqrt(lengthSq);
if (length < units)
{
from.X = to.X;
from.Y = to.Y;
return;
}
// Normalize the direction vector to get a unit vector
float unitDeltaX = deltaX / length;
float unitDeltaY = deltaY / length;
// Calculate the new coordinates
float newX = from.X + unitDeltaX * units;
float newY = from.Y + unitDeltaY * units;
// Set the new coordinates for 'from' (replace the below lines with your actual code)
from.X = (int)newX;
from.Y = (int)newY;
}
/*public static Location GetValidRandomLocation(ILocatable start, int maxRange)
{
var x = Randomizer.Get.Next(Math.Max(start.X - maxRange, 0), Math.Min(start.X + maxRange, 16000));
var y = Randomizer.Get.Next(Math.Max(start.Y - maxRange, 0), Math.Min(start.Y + maxRange, 9000));
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
public static int[] FibonacciNumbers = new int[]
{
1, 2, 3, 5, 8, 13, 21, 34, 55, 89, 144, 233, 377, 610, 987, 1597, 2584, 4181, 6765, 10946, 17711,
28657, 46368, 75025
};
public static int[] Score = new int[]
{
0,
10,
40,
90,
160,
250,
360,
490,
640,
810,
1000,
1210,
1440,
1690,
1960,
2250,
2560,
2890,
3240,
3610,
4000,
};
}
}
