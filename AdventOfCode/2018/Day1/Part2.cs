using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace AdventOfCode2018.Day1
{
    public class Part2
    {
        public static void Run()
        {
            var input = File.ReadAllLines("2018/Day1/Input.txt");
            var numbers = input.Select(i => int.Parse(i)).ToList();
            var frequencies = new HashSet<int>();
            var currentNumber = 0;
            var finished = false;

            while (!finished)
            {
                foreach (var number in numbers)
                {
                    currentNumber += number;
                    if (frequencies.Contains(currentNumber))
                    {
                        Console.WriteLine(currentNumber);
                        finished = true;
                        break;
                    }
                    else
                    {
                        frequencies.Add(currentNumber);
                    }
                }
            }
        }
    }
}
