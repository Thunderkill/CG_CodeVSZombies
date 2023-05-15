namespace CG_CodeVsZombies2
{
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
}