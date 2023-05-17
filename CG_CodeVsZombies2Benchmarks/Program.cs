// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Running;
using CG_CodeVsZombies2;
using CG_CodeVsZombies2.Utils;
using CG_CodeVsZombies2Benchmarks;

var random = new Random(1);

Console.WriteLine(random.Next(0, 10));
Console.WriteLine(random.Next(0, 10));
Console.WriteLine(random.Next(0, 10));
Console.WriteLine(random.Next(0, 10));
Console.WriteLine(random.Next(0, 10));
Console.WriteLine(random.Next(0, 10));

//BenchmarkRunner.Run<Benchmarks>();