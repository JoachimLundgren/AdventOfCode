using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;

namespace AdventOfCode.Day18
{
    public class Part1
    {
        public static void Run()
        {
            var input = File.ReadAllLines("Day18/Input.txt");
            var blue = input.Select(line => line.ToArray()).ToArray();
            var green = input.Select(line => line.ToArray()).ToArray();

            for (int i = 0; i < 10; i++)
            {
                var current = i % 2 == 1 ? blue : green;
                var other = i % 2 == 1 ? green : blue;
                for (int y = 0; y < current.Length; y++)
                {
                    for (int x = 0; x < current[0].Length; x++)
                    {
                        var adjacent = GetAdjacentChars(other, y, x);
                        if (other[y][x] == '.')
                        {
                            current[y][x] = adjacent.Count(c => c == '|') >= 3 ? '|' : '.';
                        }
                        else if(other[y][x] == '|')
                        {
                            current[y][x] = adjacent.Count(c => c == '#') >= 3 ? '#' : '|';
                        }
                        else if (other[y][x] == '#')
                        {
                            current[y][x] = adjacent.Count(c => c == '#') >= 1 && adjacent.Count(c => c == '|') >= 1 ? '#' : '.';
                        }
                    }
                }
            }

            var wood = blue.Sum(row => row.Count(c => c == '|'));
            var lumber = blue.Sum(row => row.Count(c => c == '#'));

            Console.WriteLine(wood*lumber);
        }

        private static void DebugPrint(char[][] map)
        {
            var str = new StringBuilder();
            foreach (var line in map)
            {
                str.AppendLine(string.Join("", line));
            }
            Console.WriteLine(str);
        }

        private static List<char> GetAdjacentChars(char[][] area, int y, int x)
        {
            var arr = new List<char>();

            if (y > 0)
            {
                if (x > 0)
                    arr.Add(area[y - 1][x - 1]);
                arr.Add(area[y - 1][x]);
                if (x < area[0].Length - 1)
                    arr.Add(area[y - 1][x + 1]);
            }

            if (x > 0)
                arr.Add(area[y][x - 1]);

            if (x < area[0].Length - 1)
                arr.Add(area[y][x + 1]);

            if (y < area.Length - 1)
            {
                if (x > 0)
                    arr.Add(area[y + 1][x - 1]);
                arr.Add(area[y + 1][x]);
                if (x < area[0].Length - 1)
                    arr.Add(area[y + 1][x + 1]);
            }

            return arr;
        }
    }
}
