using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace AdventOfCode2019.Day8
{
    public class Part2
    {
        public static void Run()
        {
            var input = File.ReadAllLines("2019/Day8/Input.txt").Single();
            var layerSize = 25 * 6;
            var layers = Enumerable.Range(0, input.Length / layerSize)
                .Select(i => input.Substring(i * layerSize, layerSize)).ToList();

            for (int row = 0; row < 6; row++)
            {
                for (int column = 0; column < 25; column++)
                {
                    var finalPixel = 'O';
                    foreach (var layer in layers)
                    {
                        var pixel = layer[row * 25 + column];
                        if (pixel != '2')
                        {
                            finalPixel = pixel == '0' ? 'X' : ' ';
                            break;
                        }
                    }
                    Console.Write(finalPixel);
                }
                Console.WriteLine();
            }
        }
    }
}
