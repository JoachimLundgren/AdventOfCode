using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;

namespace AdventOfCode2018.Day19
{
    public class Part1
    {
        private static Regex numbersRegex = new Regex(@"\d+");
        private static int[] registers = new[] { 0, 0, 0, 0, 0, 0 };
        public static void Run()
        {
            var input = File.ReadAllLines("2018/Day19/Input.txt");
            var ipp = -1;
            var instructions = new Dictionary<int, Instruction>();
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i].StartsWith("#ip"))
                    ipp = int.Parse(input[i].Substring(input[i].Length-1, 1));
                else
                    instructions.Add(i - 1, new Instruction(input[i]));
            }

            var ip = 0;
            while (ip < instructions.Count)
            {
                registers[ipp] = ip;
                var instruction = instructions[ip];

                switch (instruction.Operation)
                {
                    case "addr": registers[instruction.C] = registers[instruction.A] + registers[instruction.B]; break;
                    case "addi": registers[instruction.C] = registers[instruction.A] + instruction.B; break;
                    case "mulr": registers[instruction.C] = registers[instruction.A] * registers[instruction.B]; break;
                    case "muli": registers[instruction.C] = registers[instruction.A] * instruction.B; break;
                    case "banr": registers[instruction.C] = registers[instruction.A] & registers[instruction.B]; break;
                    case "bani": registers[instruction.C] = registers[instruction.A] & instruction.B; break;
                    case "borr": registers[instruction.C] = registers[instruction.A] | registers[instruction.B]; break;
                    case "bori": registers[instruction.C] = registers[instruction.A] | instruction.B; break;
                    case "setr": registers[instruction.C] = registers[instruction.A]; break;
                    case "seti": registers[instruction.C] = instruction.A; break;
                    case "gtir": registers[instruction.C] = instruction.A > registers[instruction.B] ? 1 : 0; break;
                    case "gtri": registers[instruction.C] = registers[instruction.A] > instruction.B ? 1 : 0; break;
                    case "gtrr": registers[instruction.C] = registers[instruction.A] > registers[instruction.B] ? 1 : 0; break;
                    case "eqir": registers[instruction.C] = instruction.A == registers[instruction.B] ? 1 : 0; break;
                    case "eqri": registers[instruction.C] = registers[instruction.A] == instruction.B ? 1 : 0; break;
                    case "eqrr": registers[instruction.C] = registers[instruction.A] == registers[instruction.B] ? 1 : 0; break;

                    default: throw new NotImplementedException();
                }
                ip = registers[ipp];
                ip++;

                Console.WriteLine(string.Join(", ", registers));
            }
        }

        private class Instruction
        {
            public string Operation { get; }
            public int A { get; }
            public int B { get; }
            public int C { get; }

            public Instruction(string input)
            {
                Operation = input.Substring(0, 4);
                var numbers = numbersRegex.Matches(input);
                A = int.Parse(numbers[0].Value);
                B = int.Parse(numbers[1].Value);
                C = int.Parse(numbers[2].Value);
            }
        }
    }
}
