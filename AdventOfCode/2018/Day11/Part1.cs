using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;

namespace AdventOfCode2018.Day11
{
    public class Part1
    {
        public static void Run()
        {
            var input = 7689;

            var grid = new int[301, 301];
            for (int i = 1; i <= 300; i++)
            {
                for (int j = 1; j <= 300; j++)
                {
                    var rackId = i + 10;
                    var powerLevel = rackId * j;
                    powerLevel += input;
                    powerLevel *= rackId;
                    powerLevel = (powerLevel % 1000) / 100;
                    powerLevel -= 5;
                    grid[i, j] = powerLevel;
                }
            }

            var bestx = 0;
            var besty = 0;
            var bestSum = 0;
            for (int i = 1; i < 300 - 1; i++)
            {
                for (int j = 1; j < 300 - 1; j++)
                {
                    var sum = grid[i, j] + grid[i, j + 1] + grid[i, j + 2]
                        + grid[i + 1, j] + grid[i + 1, j + 1] + grid[i + 1, j + 2]
                        + grid[i + 2, j] + grid[i + 2, j + 1] + grid[i + 2, j + 2];

                    if (sum > bestSum)
                    {
                        bestSum = sum;
                        bestx = i;
                        besty = j;
                    }
                }
            }

            Console.WriteLine($"{bestx},{besty}");
        }
    }
}
