using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Drawing;

namespace AdventOfCode.Day14
{
    public class Part1
    {
        public static void Run()
        {
            var input = int.Parse(File.ReadAllText("Day14/Input.txt"));
            var scoreboard = new List<int>() { 3, 7 };

            var elf1Index = 0;
            var elf2Index = 1;

            while (scoreboard.Count <= input + 10)
            {
                var nextNumber = scoreboard[elf1Index] + scoreboard[elf2Index];
                if (nextNumber > 9)
                    scoreboard.Add(1);
                scoreboard.Add(nextNumber % 10);

                elf1Index = (elf1Index + scoreboard[elf1Index] + 1) % scoreboard.Count;
                elf2Index = (elf2Index + scoreboard[elf2Index] + 1) % scoreboard.Count;
            }
            var result = scoreboard.Skip(input).Take(10);
            Console.WriteLine(string.Join("", result));
        }
    }
}
