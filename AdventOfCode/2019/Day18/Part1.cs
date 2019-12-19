using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Utils;

namespace AdventOfCode2019.Day18
{
    public class Part1
    {
        public void Run()
        {
            var input = File.ReadAllLines("2019/Day18/Input.txt");
            var map = input.Select(line => line.ToCharArray()).ToArray();
            var keys = map.Sum(line => line.Count(c => char.IsLower(c)));

            Coordinate currentCoordinate = null;
            for (int y = 0; y < input.Length; y++)
            {
                var x = input[y].IndexOf('@');
                if (x != -1)
                {
                    currentCoordinate = new Coordinate(x, y);
                    map[y][x] = '.';
                    break;
                }
            }

            var length = WalkTheWay(currentCoordinate, Copy(map), keys);
            Console.WriteLine(length);
        }

        private int WalkTheWay(Coordinate current, char[][] map, int keys)
        {
            var moves = 0;

            while (keys > 0)
            {
                var c = map[current.Y][current.X];

                if (char.IsLower(c))
                {
                    Unlock(c, map);
                    keys--;
                    if (keys == 0)
                        return moves;
                }
                else
                {
                    map[current.Y][current.X] = ','; //Don't go back!
                }

                moves++;
                var possibleMoves = GetPossibleMoves(current, map);
                if (!possibleMoves.Any())
                    return 1000000; //wrong way!

                if (possibleMoves.Count == 1)
                {
                    current = possibleMoves.Single();
                }
                else
                {
                    var bestCost = 1000000;
                    foreach (var move in possibleMoves)
                    {
                        var newCost = WalkTheWay(move, Copy(map), keys);

                        if (newCost < bestCost)
                            bestCost = newCost;
                    }

                    return moves + bestCost;
                }
            }

            return moves;
        }

        private char[][] Copy(char[][] map)
        {
            return map.Select(m => m.ToArray()).ToArray();
            //foreach (var row in map)
            //{
            //    for (int i = 0; i < row.Length; i++)
            //    {
            //        if (row[i] == ',')
            //            row[i] = '.';
            //    }
            //}
            //return copy;
        }

        private void Unlock(char key, char[][] map)
        {
            var door = char.ToUpper(key);
            foreach (var row in map)
            {
                for (int i = 0; i < row.Length; i++)
                {
                    if (row[i] == key || row[i] == door || row[i] == ',')
                        row[i] = '.';
                }
            }
        }


        private List<Coordinate> GetPossibleMoves(Coordinate current, char[][] map)
        {
            var moves = new List<Coordinate>();

            if (current.Y > 0 && CanMove(map[current.Y - 1][current.X]))
                moves.Add(new Coordinate(current.X, current.Y - 1));
            if (current.Y < map.Length && CanMove(map[current.Y + 1][current.X]))
                moves.Add(new Coordinate(current.X, current.Y + 1));
            if (current.X > 0 && CanMove(map[current.Y][current.X - 1]))
                moves.Add(new Coordinate(current.X - 1, current.Y));
            if (current.X < map.First().Length && CanMove(map[current.Y][current.X + 1]))
                moves.Add(new Coordinate(current.X + 1, current.Y));

            return moves;
        }

        private bool CanMove(char c/*, List<char> keys*/)
        {
            return c == '.'|| char.IsLower(c)/*  || keys.Contains(char.ToLower(c))*/;
        }
    }
}