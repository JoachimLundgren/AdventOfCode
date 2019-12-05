using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;

namespace AdventOfCode2019.Day5
{
    public class Part1
    {
        public static void Run()
        {
            var input = File.ReadAllLines("2019/Day5/Input.txt");

            var inputValue = 1;
            var outputValue = 0;
            var numbers = input.First().Split(',').Select(int.Parse).ToList();


            var running = true;
            var pointer = 0;

            while (running)
            {
                var op = numbers[pointer] % 100;
                var a = numbers[pointer] / 10000 % 10;
                var b = numbers[pointer] / 1000 % 10;
                var c = numbers[pointer] / 100 % 10;
                if (op == 1)
                {
                    numbers[a == 0 ? numbers[pointer + 3] : pointer + 3] = GetValue(numbers, c, pointer + 1) + GetValue(numbers, b, pointer + 2);
                    pointer += 4;
                }
                else if (op == 2)
                {
                    numbers[a == 0 ? numbers[pointer + 3] : pointer + 3] = GetValue(numbers, c, pointer + 1) * GetValue(numbers, b, pointer + 2);
                    pointer += 4;
                }
                else if (op == 3)
                {
                    numbers[numbers[pointer + 1]] = inputValue;
                    Console.WriteLine("Input value set");
                    pointer += 2;
                }
                else if (op == 4)
                {
                    outputValue = numbers[numbers[pointer + 1]];
                    Console.WriteLine($"Output value set to {outputValue}");
                    pointer += 2;
                }
                else if (op == 99)
                {
                    running = false;
                }
                else
                {
                    throw new ApplicationException("I fucked up");
                }
            }
        }

        private static int GetValue(List<int> array, int mode, int pointer)
        {
            var value = array[pointer];

            if (mode == 0) //Position
                return array[value];
            else if (mode == 1) //immediate 
                return value;
            else
                throw new ApplicationException($"{mode} is not a valid mode");

        }
    }
}
