using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace AdventOfCode2019.Day7
{
    public class Amplifier
    {
        private int inputPointer;

        public int Index { get; set; }
        public int Pointer { get; set; }
        public List<int> Program { get; set; }
        public bool Finished { get; set; }
        public List<int> Inputs { get; set; }

        public int NextInput => Inputs[inputPointer++];

    }
    public class Part2
    {
        public static void Run()
        {
            var input = File.ReadAllLines("2019/Day7/Input.txt");
            var program = input.First().Split(',').Select(int.Parse).ToList();
            var output = 0;

            foreach (var phase in GeneratePhases(5, 9))
            {
                var amplifiers = Enumerable.Range(0, 5).Select(val => new Amplifier { Index = val, Program = program.ToList(), Inputs = new List<int>() { phase[val] } }).ToList();
                amplifiers[0].Inputs.Add(0);
                var nextInput = 0;
                while (!amplifiers.First().Finished)    //Assumption: All finish at the same time
                {
                    for (int i = 0; i < amplifiers.Count; i++)
                    {
                        nextInput = RunCode(amplifiers[i]);
                        if (nextInput > output)
                            output = nextInput;
                        amplifiers[(i + 1) % 5].Inputs.Add(nextInput);
                    }
                }
            }
            Console.WriteLine($"Highest output: {output}");
        }


        private static int RunCode(Amplifier amp)
        {
            var running = true;
            var outputValue = 0;

            while (running)
            {
                var op = amp.Program[amp.Pointer] % 100;
                var a = amp.Program[amp.Pointer] / 10000 % 10;
                var b = amp.Program[amp.Pointer] / 1000 % 10;
                var c = amp.Program[amp.Pointer] / 100 % 10;
                if (op == 1)
                {
                    amp.Program[a == 0 ? amp.Program[amp.Pointer + 3] : amp.Pointer + 3] = GetValue(amp.Program, c, amp.Pointer + 1) + GetValue(amp.Program, b, amp.Pointer + 2);
                    amp.Pointer += 4;
                }
                else if (op == 2)
                {
                    amp.Program[a == 0 ? amp.Program[amp.Pointer + 3] : amp.Pointer + 3] = GetValue(amp.Program, c, amp.Pointer + 1) * GetValue(amp.Program, b, amp.Pointer + 2);
                    amp.Pointer += 4;
                }
                else if (op == 3)
                {
                    amp.Program[amp.Program[amp.Pointer + 1]] = amp.NextInput;
                    //Console.WriteLine("Input value set");
                    amp.Pointer += 2;
                }
                else if (op == 4)
                {
                    outputValue = amp.Program[amp.Program[amp.Pointer + 1]];
                    //Console.WriteLine($"Output value set to {outputValue}");
                    amp.Pointer += 2;
                    running = false;
                }
                else if (op == 5)
                {
                    if (GetValue(amp.Program, c, amp.Pointer + 1) != 0)
                        amp.Pointer = GetValue(amp.Program, b, amp.Pointer + 2);
                    else
                        amp.Pointer += 3;
                }
                else if (op == 6)
                {
                    if (GetValue(amp.Program, c, amp.Pointer + 1) == 0)
                        amp.Pointer = GetValue(amp.Program, b, amp.Pointer + 2);
                    else
                        amp.Pointer += 3;
                }
                else if (op == 7)
                {
                    amp.Program[a == 0 ? amp.Program[amp.Pointer + 3] : amp.Pointer + 3] = GetValue(amp.Program, c, amp.Pointer + 1) < GetValue(amp.Program, b, amp.Pointer + 2) ? 1 : 0;
                    amp.Pointer += 4;
                }
                else if (op == 8)
                {
                    amp.Program[a == 0 ? amp.Program[amp.Pointer + 3] : amp.Pointer + 3] = GetValue(amp.Program, c, amp.Pointer + 1) == GetValue(amp.Program, b, amp.Pointer + 2) ? 1 : 0;
                    amp.Pointer += 4;
                }
                else if (op == 99)
                {
                    amp.Finished = true;
                    running = false;
                }
                else
                {
                    throw new ApplicationException("I fucked up");
                }
            }

            if (amp.Program[amp.Pointer] % 100 == 99) //Is next inst halt?
                amp.Finished = true;

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

        private static List<List<int>> GeneratePhases(int min, int max)
        {
            //return new List<List<int>>() { new List<int> { 9, 8, 7, 6, 5 } };

            var phases = new List<List<int>>();
            for (int a = min; a <= max; a++)
            {
                for (int b = min; b <= max; b++)
                {
                    for (int c = min; c <= max; c++)
                    {
                        for (int d = min; d <= max; d++)
                        {
                            for (int e = min; e <= max; e++)
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
