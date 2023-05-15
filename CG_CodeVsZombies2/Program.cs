using System;

namespace CG_CodeVsZombies2
{
    internal class Program
    {
        public static void Main()
        {
            string[] inputs;

            Player player = new Player(0, 0);
            Game game = new Game(player);

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

                Console.WriteLine("0 0"); // Your destination coordinates
            }
        }
    }
}