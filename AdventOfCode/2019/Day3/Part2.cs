using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace AdventOfCode2019.Day3
{
    public class Part2
    {
        public static void Run()
        {
            var input = File.ReadAllLines("2019/Day3/Input.txt");
            var numbers = input.First().Split(',').Select(int.Parse).ToList();


            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    Test(numbers.ToList(), i, j);
                }
            }

        }

        private static void Test(List<int> numbers, int noun, int verb)
        {
            numbers[1] = noun;
            numbers[2] = verb;

            for (int i = 0; i < numbers.Count; i += 4)
            {
                var op = numbers[i];
                if (op == 1)
                {
                    numbers[numbers[i + 3]] = numbers[numbers[i + 1]] + numbers[numbers[i + 2]];
                }
                else if (op == 2)
                {
                    numbers[numbers[i + 3]] = numbers[numbers[i + 1]] * numbers[numbers[i + 2]];
                }
                else if (op == 99)
                {
                    break;
                }
                else
                {
                    throw new ApplicationException("I fucked up");
                }
            }

            Console.WriteLine($"noun: {noun} verb: {verb} result: {numbers[0]}");
        }
    }
}
