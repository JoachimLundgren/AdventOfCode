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
    public class Part2
    {
        private Dictionary<int, char[][]> maps;
        public void Run()
        {
            var input = File.ReadAllLines("2019/Day24/Input.txt");
            var map = input.Select(row => row.ToArray()).ToArray();

            map[2][2] = '?';

            maps = new Dictionary<int, char[][]>();
            maps.Add(0, map);

            for (int i = 0; i < 200; i++)
            {
                CreateNextLevels();
                var newDictionary = new Dictionary<int, char[][]>();

                foreach (var level in maps)
                {
                    var newMap = GetNextMap(level.Value, level.Key);
                    newDictionary.Add(level.Key, newMap);
                }
                maps = newDictionary;
            }

            /*foreach (var m in maps.OrderBy(kvp => kvp.Key))
            {
                Console.WriteLine(m.Key);
                m.Value.PrintMap();
                Console.WriteLine();
            } */

            var bugs = maps.Sum(m => m.Value.Sum(row => row.Count(c => c == '#')));
            Console.WriteLine(bugs); //1950 too high
        }

        private char[][] GetNextMap(char[][] map, int level)
        {
            var nextMap = new char[map.Length][];
            for (int y = 0; y < map.Length; y++)
            {
                nextMap[y] = new char[map[y].Length];
                for (int x = 0; x < map[y].Length; x++)
                {
                    if (map[y][x] == '?')
                    {
                        nextMap[y][x] = '?';
                    }
                    else
                    {
                        var adjacentBugs = GetAdjacentBugs(map, level, x, y);
                        if (map[y][x] == '#')
                            nextMap[y][x] = adjacentBugs == 1 ? '#' : '.';
                        else
                            nextMap[y][x] = adjacentBugs == 1 || adjacentBugs == 2 ? '#' : '.';
                    }
                }
            }
            return nextMap;
        }

        private void CreateNextLevels() //Flip these??
        {
            var max = maps.MaxBy(m => m.Key);
            var hasOuterBugs = max.Value.First().Any(c => c == '#')
                || max.Value.Any(row => row.First() == '#' || row.Last() == '#')
                || max.Value.Last().Any(c => c == '#');

            if (hasOuterBugs)
                maps.Add(max.Key + 1, GetEmptyMap(max.Value.Length, max.Value[0].Length));


            var min = maps.MinBy(m => m.Key);
            var hasInnerBugs = min.Value[2][1] == '#'
                            || min.Value[1][2] == '#'
                            || min.Value[3][2] == '#'
                            || min.Value[2][3] == '#';

            if (hasInnerBugs)
                maps.Add(min.Key - 1, GetEmptyMap(min.Value.Length, min.Value[0].Length));
        }

        private char[][] GetEmptyMap(int x, int y)
        {
            var newMap = new char[y][];
            for (int row = 0; row < y; row++)
            {
                newMap[row] = new char[x];
            }

            newMap[2][2] = '?';
            return newMap;
        }

        private int GetAdjacentBugs(char[][] map, int level, int x, int y)
        {
            var i = 0;
            if (maps.ContainsKey(level - 1))
            {
                if (y == 0) //ABCDE
                    i += maps[level - 1][1][2] == '#' ? 1 : 0; //8
                if (x == 0) //AFKPU
                    i += maps[level - 1][2][1] == '#' ? 1 : 0; //12
                if (y == map.Length - 1)    //UVWXY
                    i += maps[level - 1][3][2] == '#' ? 1 : 0; //18
                if (x == map[y].Length - 1) //EJOTY
                    i += maps[level - 1][2][3] == '#' ? 1 : 0; //14
            }

            if (maps.ContainsKey(level + 1))
            {
                if (x == 2 && y == 1) //H
                    i += maps[level + 1].First().Count(c => c == '#');
                if (x == 1 && y == 2) //L
                    i += maps[level + 1].Count(row => row.First() == '#');
                if (x == 3 && y == 2) //N
                    i += maps[level + 1].Count(row => row.Last() == '#');
                if (x == 2 && y == 3) //R
                    i += maps[level + 1].Last().Count(c => c == '#');
            }


            i += y > 0 && map[y - 1][x] == '#' ? 1 : 0;
            i += x > 0 && map[y][x - 1] == '#' ? 1 : 0;
            i += y < map.Length - 1 && map[y + 1][x] == '#' ? 1 : 0;
            i += x < map[y].Length - 1 && map[y][x + 1] == '#' ? 1 : 0;

            return i;
        }
    }
}