using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using System.Drawing;

namespace AdventOfCode.Day18
{
    public class Part2
    {
        public static void Run()
        {
            var input = File.ReadAllLines("Day18/Input.txt");
            var blue = input.Select(line => line.ToArray()).ToArray();
            var green = input.Select(line => line.ToArray()).ToArray();

            var history = new Dictionary<int, Tuple<int, int>>();

            for (int i = 0; i < 1000000000; i++)
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
                        else if (other[y][x] == '|')
                        {
                            current[y][x] = adjacent.Count(c => c == '#') >= 3 ? '#' : '|';
                        }
                        else if (other[y][x] == '#')
                        {
                            current[y][x] = adjacent.Count(c => c == '#') >= 1 && adjacent.Count(c => c == '|') >= 1 ? '#' : '.';
                        }
                    }
                }

                var hash = GetHashCode(current);
                if (history.ContainsKey(hash))
                {
                    var index = history[hash].Item1;
                    var count = history.Count() - index;

                    var offsetFromIndex = (1000000000 - index) % count;

                    var value = history.Values.Single(tuple => tuple.Item1 == index + offsetFromIndex - 1).Item2;
                    Console.WriteLine(value);
                    break;
                }
                else
                {
                    var wood = current.Sum(row => row.Count(c => c == '|'));
                    var lumber = current.Sum(row => row.Count(c => c == '#'));
                    history.Add(hash, new Tuple<int, int>(i, wood * lumber));
                }
            }
        }

        private static int GetHashCode(char[][] map)
        {
            int hash = 17;
            for (int i = 0; i < map.Length; i++)
            {
                for (int j = 0; j < map[0].Length; j++)
                {
                    hash = hash * 31 + map[i][j];
                }
            }
            return hash;
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
