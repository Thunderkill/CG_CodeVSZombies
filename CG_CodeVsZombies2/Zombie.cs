namespace CG_CodeVsZombies2
{
    public class Zombie : ILocatable, IIdentifiable
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
    }
}

