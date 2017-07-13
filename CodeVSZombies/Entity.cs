using System.Collections.Generic;

namespace CodeVSZombies
{
    public class Entity : Locatable
    {
        public int id;
        public Entity(int id, int x, int y) : base(x, y)
        {
            this.id = id;
        }
        public Entity(int x, int y) : base(x, y) { }


        public Entity GetClosest(List<Entity> entities)
        {
            double closest = 999;
            Entity closestEnt = null;
            for (int i = 0; i < entities.Count; i++)
            {
                Entity entity = entities[i];
                double distance = entity.DistanceTo(this);
                if (closest == 999 || distance < closest)
                {
                    closestEnt = entity;
                    closest = distance;
                }
            }
            return closestEnt;
        }
    }
}