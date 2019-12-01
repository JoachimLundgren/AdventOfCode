using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace AdventOfCode2019
{
    public class _2019
    {
        public static void Run()
        {
            Console.WriteLine("Running 2019!");
            Console.WriteLine("============");
            var stopwatch = new Stopwatch();
            stopwatch.Start();


            Day2.Part1.Run();
            Day2.Part2.Run();
            Console.WriteLine($"{stopwatch.Elapsed.TotalMilliseconds}ms");
            Console.WriteLine();
            stopwatch.Restart();


            Console.WriteLine($"2019 complete!");
            Console.ReadLine();
        }
    }
}
