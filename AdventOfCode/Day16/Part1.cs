using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Drawing;
using System.Text.RegularExpressions;

namespace AdventOfCode.Day16
{
    public class Part1
    {
        private static Regex numbersRegex = new Regex(@"\d+");
        public static void Run()
        {
            var input = File.ReadAllLines("Day16/Input.txt");

            var samples = new List<Sample>();

            for (int i = 0; i < input.Length; i++)
            {
                if (input[i].StartsWith("Before:"))
                {
                    samples.Add(new Sample(input[i], input[i + 1], input[i + 2]));
                }
            }
            var result = samples.Count(s => s.GetPossibleOperations().Count >= 3);
            Console.WriteLine(result);
        }

        private class Sample
        {
            public int[] Before { get; set; }
            public int[] Op { get; set; }
            public int[] After { get; set; }

            public Sample(string before, string op, string after)
            {
                Before = CreateArray(before);
                Op = CreateArray(op);
                After = CreateArray(after);
            }

            private int[] CreateArray(string str)
            {
                var match = numbersRegex.Matches(str);
                return new int[] { int.Parse(match[0].Value), int.Parse(match[1].Value), int.Parse(match[2].Value), int.Parse(match[3].Value) };
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
