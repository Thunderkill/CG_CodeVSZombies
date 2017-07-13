namespace CodeVSZombies
{
    public class Zombie : Entity
    {
        public Locatable nextPosition;
        public Zombie(int id, int x, int y, int nextX, int nextY) : base(id, x, y)
        {
            this.nextPosition = new Locatable(nextX, nextY);
        }
    }
}