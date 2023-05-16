// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Running;
using CG_CodeVsZombies2;
using CG_CodeVsZombies2.Utils;
using CG_CodeVsZombies2Benchmarks;


BenchmarkRunner.Run<Benchmarks>();