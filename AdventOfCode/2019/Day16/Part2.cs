using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdventOfCode.Utils;

namespace AdventOfCode2019.Day16
{
    public class Part2
    {
        public void Run()
        {
            var input = File.ReadAllLines("2019/Day16/Input.txt");
            var signal = input.Single().Select(c => (int)char.GetNumericValue(c)).ToList();
            var messageOffset = signal.Take(7).Select((t, i) => t * Convert.ToInt32(Math.Pow(10, 7 - i - 1))).Sum();

            signal = Enumerable.Repeat(signal, 10000).SelectMany(i => i).ToList();
            signal = signal.Skip(messageOffset).ToList();

            for (int i = 0; i < 100; i++)
            {
                var output = new List<int>() { signal.Last() };

                for (int a = signal.Count - 2; a >= 0; a--)
                    output.Add((signal[a] + output.Last()) % 10);

                output.Reverse();
                signal = output.ToList();
            }

            Console.WriteLine(string.Join("", signal.Take(8)));
        }
    }
}