using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdventOfCode.Utils;

namespace AdventOfCode2019.Day24
{
    public class Part1
    {
        public void Run()
        {
            var input = File.ReadAllLines("2019/Day24/Input.txt");
            var map = input.Select(row => row.ToArray()).ToArray();

            var values = new HashSet<int>();

            var finished = false;
            while (!finished)
            {
                map = GetNextMap(map);
                //map.PrintMap();
                //Console.WriteLine();

                var points = CalcPoints(map);
                if (values.Contains(points))
                {
                    Console.WriteLine(points);
                    finished = true;
                }
                else
                {
                    values.Add(points);
                }
            }
        }

        private char[][] GetNextMap(char[][] map)
        {
            var nextMap = new char[map.Length][];
            for (int y = 0; y < map.Length; y++)
            {
                nextMap[y] = new char[map[y].Length];
                for (int x = 0; x < map[y].Length; x++)
                {
                    var adjacentBugs = GetAdjacentBugs(map, x, y);
                    if (map[y][x] == '#')
                        nextMap[y][x] = adjacentBugs == 1 ? '#' : '.';
                    else
                        nextMap[y][x] = adjacentBugs == 1 || adjacentBugs == 2 ? '#' : '.';
                }
            }
            return nextMap;
        }

        private int GetAdjacentBugs(char[][] map, int x, int y)
        {
            var i = 0;

            i += y > 0 && map[y - 1][x] == '#' ? 1 : 0;
            i += x > 0 && map[y][x - 1] == '#' ? 1 : 0;
            i += y < map.Length - 1 && map[y + 1][x] == '#' ? 1 : 0;
            i += x < map[y].Length - 1 && map[y][x + 1] == '#' ? 1 : 0;

            return i;
        }

        private int CalcPoints(char[][] map)
        {
            var value = 0;
            var nextValue = 1;
            for (int y = 0; y < map.Length; y++)
            {
                for (int x = 0; x < map[y].Length; x++)
                {
                    value += map[y][x] == '#' ? nextValue : 0;
                    if (nextValue == 1)
                        nextValue = 2;
                    else
                        nextValue *= 2;
                }
            }
            return value;
        }
    }
}