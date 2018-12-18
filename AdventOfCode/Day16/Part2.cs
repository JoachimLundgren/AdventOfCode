using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode.Day16
{
    public class Part2
    {
        private static Regex numbersRegex = new Regex(@"\d+");
        private static Dictionary<int, string> operationTranslation;
        public static void Run()
        {
            var input = File.ReadAllLines("Day16/Input.txt");

            var samples = new List<Sample>();
            var operations = new List<int[]>();

            for (int i = 0; i < input.Length; i++)
            {
                if (input[i].StartsWith("Before:"))
                {
                    samples.Add(new Sample(input[i], input[i + 1], input[i + 2]));
                }
            }

            for (int i = 3226; i < input.Length; i++)
            {
                if (!string.IsNullOrEmpty(input[i]) && char.IsDigit(input[i][0]))
                {
                    operations.Add(CreateArray(input[i]));
                }
            }

            operationTranslation = new Dictionary<int, string>();

            while (samples.Any())
            {
                var single = samples.Where(s => s.PossibleOperations.Count == 1).FirstOrDefault();
                if (single != null)
                {
                    operationTranslation.Add(single.Op[0], single.PossibleOperations.Single());
                    samples.RemoveAll(s => s.Op[0] == single.Op[0]);
                    samples.ForEach(s => s.PossibleOperations.Remove(single.PossibleOperations.Single()));
                }
                else
                {

                }
            }

            var registers = new[] { 0, 0, 0, 0 };
            foreach (var op in operations)
            {
                registers = DoOperation(registers, op);
                Console.WriteLine(string.Join(",", registers));
            }
            
            Console.WriteLine(registers[0]); //682 to high
        }

        private static int[] DoOperation(int[] registers, int[] operation)
        {
            var op = operationTranslation[operation[0]];

            switch (op)
            {
                case "addr": registers[operation[3]] = registers[operation[1]] + registers[operation[2]]; break;
                case "addi": registers[operation[3]] = registers[operation[1]] + operation[2]; break;
                case "mulr": registers[operation[3]] = registers[operation[1]] * registers[operation[2]]; break;
                case "muli": registers[operation[3]] = registers[operation[1]] * operation[2]; break;
                case "banr": registers[operation[3]] = registers[operation[1]] & registers[operation[2]]; break;
                case "bani": registers[operation[3]] = registers[operation[1]] & operation[2]; break;
                case "borr": registers[operation[3]] = registers[operation[1]] | registers[operation[2]]; break;
                case "bori": registers[operation[3]] = registers[operation[1]] | operation[2]; break;
                case "setr": registers[operation[3]] = registers[operation[1]]; break;
                case "seti": registers[operation[3]] = operation[1]; break;
                case "gtir": registers[operation[3]] = operation[1] > registers[operation[2]] ? 1 : 0; break;
                case "gtri": registers[operation[3]] = registers[operation[1]] > operation[2] ? 1 : 0; break;
                case "gtrr": registers[operation[3]] = registers[operation[1]] > registers[operation[2]] ? 1 : 0; break;
                case "eqir": registers[operation[3]] = operation[1] == registers[operation[2]] ? 1 : 0; break;
                case "eqri": registers[operation[3]] = registers[operation[1]] == operation[2] ? 1 : 0; break;
                case "eqrr": registers[operation[3]] = registers[operation[1]] == registers[operation[2]] ? 1 : 0; break;

                default: throw new NotImplementedException();
            }

            return registers;
        }


        public static int[] CreateArray(string str)
        {
            var match = numbersRegex.Matches(str);
            return new int[] { int.Parse(match[0].Value), int.Parse(match[1].Value), int.Parse(match[2].Value), int.Parse(match[3].Value) };
        }

        private class Sample
        {
            public int[] Before { get; set; }
            public int[] Op { get; set; }
            public int[] After { get; set; }
            public List<string> PossibleOperations { get; set; }

            public Sample(string before, string op, string after)
            {
                Before = CreateArray(before);
                Op = CreateArray(op);
                After = CreateArray(after);
                PossibleOperations = GetPossibleOperations();
            }

            public List<string> GetPossibleOperations()
            {
                var operations = new List<string>();

                for (int i = 0; i < 4; i++)
                {
                    if (i != Op[3] && Before[i] != After[i]) //Sanity check, only modify Register C
                        return operations;
                }

                var a = Op[1];
                var b = Op[2];
                var c = After[Op[3]];

                if (a < 4)  //a register, b value
                {
                    a = Before[Op[1]];
                    b = Op[2];

                    if (a + b == c)
                        operations.Add("addi");

                    if (a * b == c)
                        operations.Add("muli");

                    if ((a & b) == c)
                        operations.Add("bani");

                    if ((a | b) == c)
                        operations.Add("bori");

                    if (a == c)
                        operations.Add("setr");

                    if (a > b && c == 1 || a <= b && c == 0)
                        operations.Add("gtri");

                    if (a == b && c == 1 || a != b && c == 0)
                        operations.Add("eqri");

                }

                if (b < 4) //a value, b register
                {
                    a = Op[1];
                    b = Before[Op[2]];

                    if (a > b && c == 1 || a <= b && c == 0)
                        operations.Add("gtir");

                    if (a == b && c == 1 || a != b && c == 0)
                        operations.Add("eqir");

                    if (a == c)
                        operations.Add("seti");
                }

                if (a < 4 && b < 4) //a register, b register
                {
                    a = Before[Op[1]];
                    b = Before[Op[2]];

                    if (a + b == c)
                        operations.Add("addr");

                    if (a * b == c)
                        operations.Add("mulr");

                    if ((a & b) == c)
                        operations.Add("banr");

                    if ((a | b) == c)
                        operations.Add("borr");

                    if (a > b && c == 1 || a <= b && c == 0)
                        operations.Add("gtrr");

                    if (a == b && c == 1 || a != b && c == 0)
                        operations.Add("eqrr");
                }

                return operations;
            }
        }
    }
}
