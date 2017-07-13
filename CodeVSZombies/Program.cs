using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeVSZombies
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] inputs;

            // game loop
            while (true)
            {
                List<Human> humans = new List<Human>();
                List<Zombie> zombies = new List<Zombie>();
                inputs = Console.ReadLine().Split(' ');
                int x = int.Parse(inputs[0]);
                int y = int.Parse(inputs[1]);
                Ash player = new Ash(x, y);
                int humanCount = int.Parse(Console.ReadLine());
                for (int i = 0; i < humanCount; i++)
                {
                    inputs = Console.ReadLine().Split(' ');
                    int humanId = int.Parse(inputs[0]);
                    int humanX = int.Parse(inputs[1]);
                    int humanY = int.Parse(inputs[2]);
                    humans.Add(new Human(humanId, humanX, humanY));
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
                    zombies.Add(new Zombie(zombieId, zombieX, zombieY, zombieXNext, zombieYNext));
                }

                Human human = getSaveableHuman(player, humans, zombies);

                if (human != null)
                {
                    Entity closestZombie = human.GetClosest(zombies.ToList<Entity>());
                    Entity target = closestZombie ?? human;
                    Console.WriteLine(target.ToString());
                }
                else
                {
                    Console.WriteLine("0 0");
                }

                

                
            }
        }


        static Human getSaveableHuman(Ash player, List<Human> humans, List<Zombie> zombies)
        {
            for (int h = 0; h < humans.Count; h++)
            {
                Human human = humans[h];
                bool saveable = true;
                for (int z = 0; z < zombies.Count; z++)
                {
                    Zombie zombie = zombies[z];

                    double zombieTimeToHuman = (zombie.DistanceTo(human)) / 400.0f;
                    double playerTimeToZombie = (player.DistanceTo(zombie) - 2000.0f) / 1000.0f;

                    Console.Error.WriteLine("It would take me " + playerTimeToZombie + " units to kill the zombie");
                    Console.Error.WriteLine("Zombie will reach human in " + zombieTimeToHuman);

                    if (playerTimeToZombie > zombieTimeToHuman)
                    {
                        saveable = false;
                    }
                }
                if (saveable)
                    return human;
                //Entity closestZombie = human.GetClosest(zombies.ToList<Entity>());
            }
            return null;
        }
    }
}
