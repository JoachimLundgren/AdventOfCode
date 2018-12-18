using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace AdventOfCode.Day2
{
    public class Part1
    {
        public static void Run()
        {
            var input = File.ReadAllLines("Day2/Input.txt");
            var twos = 0;
            var threes = 0;

            foreach (var line in input)
            {
                var dict = new Dictionary<char, int>();
                foreach (var ch in line)
                {
                    if (dict.ContainsKey(ch))
                        dict[ch]++;
                    else
                        dict.Add(ch, 1);
                }

                if (dict.Values.Any(v => v == 2))
                    twos++;

                if (dict.Values.Any(v => v == 3))
                    threes++;
            }

            Console.WriteLine(twos * threes);
        }
    }
}
