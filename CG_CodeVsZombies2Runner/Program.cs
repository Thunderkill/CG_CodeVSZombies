// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using CliWrap;
using CliWrap.EventStream;

var gameInfo = new string[]
{
    "3989 3259",
    "4",
    "0 3647 384",
    "1 60 3262",
    "2 2391 1601",
    "3 2363 3422",
    "30",
    "0 6485 499 6085 482",
    "1 7822 446 7422 440",
    "2 9202 826 8803 794",
    "3 11060 253 10660 260",
    "4 12568 808 12183 917",
    "5 14148 650 13760 749",
    "6 6571 1893 6217 2080",
    "7 8484 2013 8098 2119",
    "8 9669 1968 9278 2056",
    "9 7570 3338 7170 3329",
    "10 9780 3611 9380 3586",
    "11 8360 4767 7981 4636",
    "12 9804 4154 9408 4093",
    "13 10935 4977 10546 48",
    "14 12310 4614 11915 45",
    "15 13891 4302 13493 42",
    "16 913 5636 777 5259",
    "17 2410 5912 2402 5512",
    "18 3952 6143 3957 5743",
    "19 4615 5995 4525 5605",
    "20 6568 6085 6298 5789",
    "21 8204 5579 7853 5386",
    "22 9049 5470 8682 5309",
    "23 30 6798 33 6398",
    "24 1798 6682 1866 6287",
    "25 3247 7664 3165 7272",
    "26 5005 7319 4907 6930",
    "27 6415 7094 6201 6755",
    "28 8159 7447 7876 7163",
    "29 9550 6847 9213 6630",
};

var finalData = new List<string>();

for (int i = 0; i < 20; i++)
{
    finalData.AddRange(gameInfo);
}


var game1 = Cli.Wrap("../../../../CG_CodeVsZombies2/bin/Release/net6.0/CG_CodeVsZombies2.exe").WithStandardInputPipe(PipeSource.FromString(String.Join("\n", finalData)));

var game1Watch = new Stopwatch();

var evolutionCounts = new List<int>();

var cts = new CancellationTokenSource();

var rounds = 0;

await foreach (var cmdEvent in game1.ListenAsync(cts.Token))
{
    switch (cmdEvent)
    {
        case StartedCommandEvent started:
            game1Watch.Restart();
            Console.WriteLine($"Process started; ID: {started.ProcessId}");
            break;
        case StandardOutputCommandEvent stdOut:
            //Console.WriteLine($"Out> {stdOut.Text}");
            rounds++;
            if (rounds >= 10)
            {
                cts.Cancel();
            }
            break;
        case StandardErrorCommandEvent stdErr:
            //Console.WriteLine($"Err> {stdErr.Text}");
            if (stdErr.Text.Contains("EVOLUTIONS: "))
            {
                var count = int.Parse(stdErr.Text.Replace("EVOLUTIONS: ", ""));
                evolutionCounts.Add(count);
                Console.WriteLine(count);
            }
            if (stdErr.Text.Contains("RANDOM:"))
            {
                Console.WriteLine(stdErr.Text);
            }
            break;
        case ExitedCommandEvent exited:
            Console.WriteLine($"Process exited; Code: {exited.ExitCode}");
            Console.WriteLine("Took " + game1Watch.ElapsedMilliseconds + "ms");
            Console.WriteLine("Total evolutions ran:");

            foreach (var count in evolutionCounts)
            {
                Console.WriteLine(count);
            }
            break;
    }
}

