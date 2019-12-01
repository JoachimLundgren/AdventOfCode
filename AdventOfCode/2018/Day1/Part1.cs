using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace AdventOfCode2018.Day1
{
    public class Part1
    {
        public static void Run()
        {
            var input = File.ReadAllLines("2018/Day1/Input.txt");
            Console.WriteLine(input.Sum(i => int.Parse(i)));
        }
    }
}
