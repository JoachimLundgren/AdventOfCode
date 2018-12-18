using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using System.Drawing;

namespace AdventOfCode.Day5
{
    public class Part2
    {
        private static int reactDiff = 'a' - 'A';
        public static void Run()
        {
            var input = File.ReadAllText("Day5/Input.txt");
            var best = input.Length;
            input = React(input, best);

            var alphabet = Enumerable.Range('A', 26).Select(i => (char)i);
            foreach (var letter in alphabet)
            {
                var newString = input.Replace(letter.ToString(), string.Empty).Replace(((char)(letter + reactDiff)).ToString(), "");

                var newValue = React(newString, best).Length;
                if (best > newValue)
                    best = newValue;
            }

            Console.WriteLine(best);
        }


        private static string React(string input, int bestValue)
        {
            for (int i = 0; i < Math.Min(input.Length, bestValue) - 1; i++)
            {
                if (Math.Abs(input[i] - input[i + 1]) == reactDiff)
                {
                    input = input.Remove(i, 2);
                    i = Math.Max(-1, i - 2);
                }
            }

            return input;
        }
    }
}
