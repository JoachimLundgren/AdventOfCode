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


            //Day2.Part1.Run();
            //Day2.Part2.Run();

            //Day3.Part1.Run();
            //Day3.Part2.Run();

            //Day4.Part1.Run();
            //Day4.Part2.Run();

            //Day5.Part1.Run();
            //Day5.Part2.Run();

            //Day6.Part1.Run();
            //Day6.Part2.Run();

            Day7.Part1.Run();
            //Day7.Part2.Run();

            Console.WriteLine($"{stopwatch.Elapsed.TotalMilliseconds}ms");
            Console.WriteLine();
            stopwatch.Restart();


            Console.WriteLine($"2019 complete!");
            Console.ReadLine();
        }
    }
}
