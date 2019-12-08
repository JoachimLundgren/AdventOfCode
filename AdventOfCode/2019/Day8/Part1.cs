using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;

namespace AdventOfCode2019.Day8
{
    public class Part1
    {
        public static void Run()
        {
            var input = File.ReadAllLines("2019/Day8/Input.txt").Single();
            var layerSize = 25 * 6;
            var layers = Enumerable.Range(0, input.Length / layerSize)
                .Select(i => input.Substring(i * layerSize, layerSize)).ToList();

            var zeros = int.MaxValue;
            var ones = 0;
            var twos = 0;

            foreach (var layer in layers)
            {
                var strZeros = layer.Count(x => x == '0');
                if (strZeros < zeros)
                {
                    zeros = strZeros;
                    ones = layer.Count(x => x == '1');
                    twos = layer.Count(x => x == '2');
                }
            }

            Console.WriteLine(ones * twos);
        }
    }
}
