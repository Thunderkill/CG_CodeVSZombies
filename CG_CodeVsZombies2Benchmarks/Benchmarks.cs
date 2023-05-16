using BenchmarkDotNet.Attributes;
using CG_CodeVsZombies2;
using CG_CodeVsZombies2.Utils;

namespace CG_CodeVsZombies2Benchmarks;

public class Benchmarks
{
    /*[Benchmark]
    public void FibonacciOriginal()
    {
        var asd = NumberUtils.Fibonacci(2);
    }*/

    [Benchmark]
    public void FibonacciPrecomputed()
    {
        var asd = NumberUtils.FibonacciNumbers[2];
    }
}