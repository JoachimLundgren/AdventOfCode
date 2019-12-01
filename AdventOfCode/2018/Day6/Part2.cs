using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using System.Drawing;

namespace AdventOfCode2018.Day6
{
    public class Part2
    {
        public static void Run()
        {
            var input = File.ReadAllLines("2018/Day6/Input.txt");
            var fixedCoordinates = input.Select(i => GetCoordinate(i)).ToList();

            var top = fixedCoordinates.Min(c => c.Y);
            var bottom = fixedCoordinates.Max(c => c.Y);
            var left = fixedCoordinates.Min(c => c.X);
            var right = fixedCoordinates.Max(c => c.X);

            var res = 0;
            for (int i = top; i <= bottom; i++)
            {
                for (int j = left; j <= right; j++)
                {
                    var sumOfDist = fixedCoordinates.Select(c => ManhattanDistance(c, j, i)).Sum();
                    if (sumOfDist < 10000)
                        res++;
                }
            }

            Console.WriteLine(res);
        }

        private static int ManhattanDistance(Point first, int x, int y)
        {
            return Math.Abs(first.X - x) +  Math.Abs(first.Y - y);
        }

        private static Point GetCoordinate(string input)
        {
            var parts = input.Split(',');
            return new Point(int.Parse(parts[0]), int.Parse(parts[1]));
        }
    }
}
