using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace AdventOfCode.Day14
{
    public class Part2
    {
        public static void Run()
        {
            var input = File.ReadAllText("Day14/Input.txt");
            var inputArray = input.Select(c => (int)char.GetNumericValue(c)).ToList();
            var scoreboard = new List<int>() { 3, 7 };

            var elf1Index = 0;
            var elf2Index = 1;

            var awesomeness = 0;
            var scoreboardIndex = 0;

            while (true)
            {
                var nextNumber = scoreboard[elf1Index] + scoreboard[elf2Index];
                if (nextNumber > 9)
                    scoreboard.Add(1);
                scoreboard.Add(nextNumber % 10);

                elf1Index = (elf1Index + scoreboard[elf1Index] + 1) % scoreboard.Count;
                elf2Index = (elf2Index + scoreboard[elf2Index] + 1) % scoreboard.Count;


                while (scoreboardIndex + awesomeness < scoreboard.Count)
                {
                    if (inputArray[awesomeness] == scoreboard[scoreboardIndex + awesomeness])
                    {
                        awesomeness++;

                        if (awesomeness == inputArray.Count)
                        {
                            Console.WriteLine(scoreboard.Count - inputArray.Count); //20279773 to high, 15563340 to low
                            return;
                        }
                    }
                    else
                    {
                        scoreboardIndex++;
                        awesomeness = 0;
                    }
                }
            }
        }
    }
}
