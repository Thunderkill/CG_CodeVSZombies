namespace CG_CodeVsZombies2
{
    public class Human : ILocatable, IIdentifiable
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
    }
}