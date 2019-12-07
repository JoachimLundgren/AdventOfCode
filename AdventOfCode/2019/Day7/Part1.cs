using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;

namespace AdventOfCode2019.Day7
{
    public class Part1
    {
        public static void Run()
        {
            var input = File.ReadAllLines("2019/Day7/Input.txt");
            var program = input.First().Split(',').Select(int.Parse).ToList();
            var output = 0;
            foreach (var phase in GeneratePhases())
            {
                var nextInput = 0;
                for (int i = 0; i < 5; i++)
                {
                    nextInput = RunCode(new List<int>() { phase[i], nextInput }, program.ToList());
                    if (nextInput > output)
                        output = nextInput;
                }

                Console.WriteLine($"[{string.Join(',', phase)}] generated {nextInput}");
            }
            Console.WriteLine($"Highest output: {output}");
        }


        private static int RunCode(List<int> inputs, List<int> program)
        {
            var running = true;
            var inputPointer = 0;
            var pointer = 0;
            var outputValue = 0;

            while (running)
            {
                var op = program[pointer] % 100;
                var a = program[pointer] / 10000 % 10;
                var b = program[pointer] / 1000 % 10;
                var c = program[pointer] / 100 % 10;
                if (op == 1)
                {
                    program[a == 0 ? program[pointer + 3] : pointer + 3] = GetValue(program, c, pointer + 1) + GetValue(program, b, pointer + 2);
                    pointer += 4;
                }
                else if (op == 2)
                {
                    program[a == 0 ? program[pointer + 3] : pointer + 3] = GetValue(program, c, pointer + 1) * GetValue(program, b, pointer + 2);
                    pointer += 4;
                }
                else if (op == 3)
                {
                    program[program[pointer + 1]] = inputs[inputPointer];
                    //Console.WriteLine("Input value set");
                    inputPointer++;
                    pointer += 2;
                }
                else if (op == 4)
                {
                    outputValue = program[program[pointer + 1]];
                    //Console.WriteLine($"Output value set to {outputValue}");
                    pointer += 2;
                }
                else if (op == 5)
                {
                    if (GetValue(program, c, pointer + 1) != 0)
                        pointer = GetValue(program, b, pointer + 2);
                    else
                        pointer += 3;
                }
                else if (op == 6)
                {
                    if (GetValue(program, c, pointer + 1) == 0)
                        pointer = GetValue(program, b, pointer + 2);
                    else
                        pointer += 3;
                }
                else if (op == 7)
                {
                    program[a == 0 ? program[pointer + 3] : pointer + 3] = GetValue(program, c, pointer + 1) < GetValue(program, b, pointer + 2) ? 1 : 0;
                    pointer += 4;
                }
                else if (op == 8)
                {
                    program[a == 0 ? program[pointer + 3] : pointer + 3] = GetValue(program, c, pointer + 1) == GetValue(program, b, pointer + 2) ? 1 : 0;
                    pointer += 4;
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

            return outputValue;
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

        private static List<List<int>> GeneratePhases()
        {
            var phases = new List<List<int>>();
            for (int a = 0; a < 5; a++)
            {
                for (int b = 0; b < 5; b++)
                {
                    for (int c = 0; c < 5; c++)
                    {
                        for (int d = 0; d < 5; d++)
                        {
                            for (int e = 0; e < 5; e++)
                            {
                                if (a != b && a != c && a != d && a != e
                                    && b != c && b != d && b != e
                                    && c != d && c != e
                                    && d != e)
                                {
                                    phases.Add(new List<int>() { a, b, c, d, e });
                                }
                            }
                        }
                    }
                }
            }

            return phases;
        }
    }
}
