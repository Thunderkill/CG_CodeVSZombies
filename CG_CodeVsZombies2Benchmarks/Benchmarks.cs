using BenchmarkDotNet.Attributes;
using CG_CodeVsZombies2;
using CG_CodeVsZombies2.Utils;

namespace CG_CodeVsZombies2Benchmarks;

public class Benchmarks
{

    private Player player1;
    private Location player2;
    
    [GlobalSetup]
    public void Setup()
    {
        player1 = new Player(0, 0);
        player2 = new Location(0, 0);
    }
    
    [Benchmark]
    public void New()
    {
        player2 = new Location(15, 15);
        var asd = player2.X;
    }

    [Benchmark]
    public void Old()
    {
        player1.X = 15;
        player1.Y = 15;
        var asd = player1.X;
    }
}