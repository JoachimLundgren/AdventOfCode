using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace AdventOfCode.Day1
{
    public class Part1
    {
        public static void Run()
        {
            var input = File.ReadAllLines("Day1/Input.txt");
            Console.WriteLine(input.Sum(i => int.Parse(i)));
        }
    }
}
