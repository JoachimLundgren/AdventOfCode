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
            var doors = map.Sum(line => line.Count(c => char.IsUpper(c)));

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


            var length = WalkTheWay(currentCoordinate, null, Copy(map), doors, new List<char>(), new List<char>());
            Console.WriteLine(length);
        }

        private int WalkTheWay(Coordinate current, Coordinate previous, char[][] map, int doors, List<char> doorsFound, List<char> keysFound)
        {
            var moves = 0;

            while (doorsFound.Count < doors)
            {
                var possibleMoves = GetPossibleMoves(current, map, keysFound);
                possibleMoves.Remove(previous);
                if (possibleMoves.Count == 0)
                {
                    return int.MaxValue; //wrong way!
                }
                else if (possibleMoves.Count == 1)
                {
                    var c = map[possibleMoves.Single().Y][possibleMoves.Single().X];
                    if (c != '.')
                    {
                        if (char.IsUpper(c))
                        {
                            doorsFound.Add(c);
                            map[possibleMoves.Single().Y][possibleMoves.Single().X] = '.';
                        }
                        else
                        {
                            keysFound.Add(c);
                            map[possibleMoves.Single().Y][possibleMoves.Single().X] = '.';
                        }
                        moves = WalkTheWay(possibleMoves.Single(), null, Copy(map), doors - doorsFound.Count, new List<char>(), keysFound.ToList());
                    }
                    previous = current;
                    current = possibleMoves.Single();
                    moves++;
                }
                else
                {
                    var shortestPath = int.MaxValue;
                    foreach (var move in possibleMoves)
                    {
                        int path;
                        var c = map[move.Y][move.X];
                        if (c != '.')
                        {
                            if (char.IsUpper(c))
                                doorsFound.Add(c);
                            else
                                keysFound.Add(c);

                            map[possibleMoves.Single().Y][possibleMoves.Single().X] = '.';
                            path = WalkTheWay(move, null, Copy(map), doors - doorsFound.Count, new List<char>(), keysFound.ToList());
                        }
                        else
                        {
                            path = WalkTheWay(move, current, Copy(map), doors - doorsFound.Count, new List<char>(), keysFound.ToList());
                        }

                        if (path < shortestPath)
                            shortestPath = path;
                    }
                    return moves + shortestPath;
                }
            }

            return moves;
        }

        private char[][] Copy(char[][] map)
        {
            return map.Select(m => m.ToArray()).ToArray();
        }


        private List<Coordinate> GetPossibleMoves(Coordinate current, char[][] map, List<char> keys)
        {
            var moves = new List<Coordinate>();

            if (current.Y > 0 && CanMove(map[current.Y - 1][current.X], keys))
                moves.Add(new Coordinate(current.X, current.Y - 1));
            if (current.Y < map.Length && CanMove(map[current.Y + 1][current.X], keys))
                moves.Add(new Coordinate(current.X, current.Y + 1));
            if (current.X > 0 && CanMove(map[current.Y][current.X - 1], keys))
                moves.Add(new Coordinate(current.X - 1, current.Y));
            if (current.X < map.First().Length && CanMove(map[current.Y][current.X + 1], keys))
                moves.Add(new Coordinate(current.X + 1, current.Y));

            return moves;
        }

        private bool CanMove(char c, List<char> keys)
        {
            return c == '.' || char.IsLower(c) || keys.Contains(char.ToLower(c));
        }
    }
}