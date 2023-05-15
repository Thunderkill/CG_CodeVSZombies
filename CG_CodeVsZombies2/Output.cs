// Author: Thunderr
using System.Collections.Generic;
using System;
namespace CG_CodeVsZombies2
{
public struct Game : IClonable<Game>
{
public Dictionary<int, Zombie> Zombies { get; set; }
public Dictionary<int, Human> Humans { get; set; }
public int Score { get; set; }
public Player Player { get; set; }
public Game(Player player)
{
Zombies = new Dictionary<int, Zombie>();
Humans = new Dictionary<int, Human>();
Player = player;
Score = 0;
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
newGame.Humans.Add(human.Key, human.Value.Clone());
}
newGame.Score = Score;
return newGame;
}
}
public class Human : ILocatable, IIdentifiable, IClonable<Human>
{
public int Id { get; set; }
public int X { get; set; }
public int Y { get; set; }
public bool Alive { get; set; }
public Human(int id, int x, int y)
{
Id = id;
X = x;
Y = y;
Alive = true;
}
private Human(int id, int x, int y, bool alive)
{
Id = id;
X = x;
Y = y;
Alive = alive;
}
public Human Clone()
{
return new Human(Id, X, Y, Alive);
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
Game simulatedGame = new Game(player);
bool simulationInitialized = false;
// game loop
while (true)
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
// Write an action using Console.WriteLine()
// To debug: Console.Error.WriteLine("Debug messages...");
if (!simulationInitialized)
{
simulatedGame = game.Clone();
simulationInitialized = true;
}
var target = new Location(0, 0);
var simulation = Simulator.Simulate(simulatedGame, target);
Console.Error.WriteLine(
$"Next round we should have {simulation.Score} score. {simulation.Humans.Count} humans alive");
Console.Error.WriteLine(
$"Player should be at {simulation.Player.X}, {simulation.Player.Y}");
simulatedGame = simulation;
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
return newGame;
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