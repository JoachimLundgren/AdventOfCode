using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace AdventOfCode2019.Day2
{
    public class Part1
    {
        public static void Run()
        {
            var input = File.ReadAllLines("2019/Day2/Input.txt");
            var numbers = input.First().Split(',').Select(int.Parse).ToList();

            numbers[1] = 12;
            numbers[2] = 2;

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
                Console.WriteLine($"{i} - {numbers[0]}");
            }

            Console.WriteLine(numbers[0]); //797870 to low
            Console.WriteLine(string.Join(", ", numbers));

        }
    }
}
