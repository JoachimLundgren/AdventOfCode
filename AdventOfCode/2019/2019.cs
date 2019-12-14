using AdventOfCode.Utils;
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

            //Day7.Part1.Run();
            //Day7.Part2.Run();

            //Day8.Part1.Run();
            //Day8.Part2.Run();

            //Day9.Part1.Run();
            //Day9.Part2.Run();

            //Day10.Part1.Run();
            //Day10.Part2.Run();

            //Day11.Part1.Run();
            //Day11.Part2.Run();

            //Day12.Part1.Run();
            //Day12.Part2.Run();

            //Day13.Part1.Run();
            //Day13.Part2.Run();
            //Day13.Game.Run();

            //Run<Day14.Part1>();
            Run<Day14.Part2>();

            Run<Day15.Part1>();
            Run<Day15.Part2>();

            Console.WriteLine($"{stopwatch.Elapsed.TotalMilliseconds}ms");
            Console.WriteLine();
            stopwatch.Restart();


            Console.WriteLine($"2019 complete!");
            Console.ReadLine();
        }

        private static void Run<T>() where T : new()
        {
            var method = typeof(T).GetMethod("Run");
            var obj = new T();
            method.Invoke(obj, new object[] { });
        }
    }
}
