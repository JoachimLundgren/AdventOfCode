using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using System.Drawing;

namespace AdventOfCode2018.Day11
{
    public class Part2
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

            var result = new int[301, 301];

            var bestx = 0;
            var besty = 0;
            var bestk = 0;
            var bestSum = 0;


            for (int k = 0; k < 300; k++)
            {
                for (int i = 1; i <= 300; i++)
                {
                    for (int j = 1; j <= 300; j++)
                    {
                        if (i + k <= 300 && j + k <= 300)
                        {
                            var sum = result[i, j];

                            for (int a = 0; a < k; a++)
                            {
                                sum += grid[i + k, j + a] + grid[i + a, j + k];
                            }
                            sum += grid[i + k, j + k];
                            result[i, j] = sum;

                            if (sum > bestSum)
                            {
                                bestSum = sum;
                                bestx = i;
                                besty = j;
                                bestk = k;
                            }
                        }
                    }
                }
            }

            Console.WriteLine($"{bestx},{besty},{bestk + 1}");
        }
    }
}
