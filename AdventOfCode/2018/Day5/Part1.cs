using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;

namespace AdventOfCode2018.Day5
{
    public class Part1
    {
        public static void Run()
        {
            var input = File.ReadAllText("2018/Day5/Input.txt");

            var reactDiff = 'a' - 'A';

            for (int i = 0; i < input.Length - 1; i++)
            {
                if (Math.Abs(input[i] - input[i + 1]) == reactDiff)
                {
                    input = input.Remove(i, 2);
                    i = Math.Max(-1, i - 2);
                }
            }


            Console.WriteLine(input.Length);
        }
    }
}
