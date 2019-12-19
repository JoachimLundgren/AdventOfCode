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
            var items = map.Sum(line => line.Count(c => char.IsUpper(c) || char.IsLower(c)));

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

            var doorsList = new List<char>();
            var length = WalkTheWay(currentCoordinate, null, Copy(map), items, ref doorsList, new List<char>());
            Console.WriteLine(length);
        }

        private int WalkTheWay(Coordinate current, Coordinate previous, char[][] map, int items, ref List<char> doorsFound, List<char> keysFound)
        {
            var moves = 0;

            while (doorsFound.Count + keysFound.Count < items)
            {
                var possibleMoves = GetPossibleMoves(current, map, keysFound);
                if (possibleMoves.Count == 1 && possibleMoves.Contains(previous))
                    return 1000000; //wrong way!


                var possibleNewMoves = possibleMoves.Except(new[] { previous }).ToList();
                if (possibleNewMoves.Count == 1)
                {
                    var c = map[possibleNewMoves.Single().Y][possibleNewMoves.Single().X];
                    if (c != '.')
                    {
                        if (char.IsUpper(c))
                        {
                            doorsFound.Add(c);
                            map[possibleNewMoves.Single().Y][possibleNewMoves.Single().X] = '.';
                        }
                        else
                        {
                            keysFound.Add(c);
                            map[possibleNewMoves.Single().Y][possibleNewMoves.Single().X] = '.';
                        }

                        var bestCost = 1000000;
                        var bestDoors = new List<char>();
                        foreach (var newMove in possibleMoves) //we could go back now!
                        {
                            var newDoors = new List<char>();
                            var newCost = WalkTheWay(newMove, null, Copy(map), items - doorsFound.Count, ref newDoors, keysFound.ToList());
                            if (newCost < bestCost)
                            {
                                bestCost += newCost;
                                bestDoors = newDoors;
                            }
                        }

                        moves += bestCost;
                        doorsFound.AddRange(bestDoors);
                    }
                    else
                    {
                        previous = current;
                        current = possibleNewMoves.Single();
                        moves++;
                    }
                }
                else
                {
                    var shortestPath = 1000000;
                    foreach (var move in possibleMoves)
                    {
                        int path;
                        var c = map[move.Y][move.X];
                        if (c != '.')
                        {
                            var newDoors = new List<char>();
                            path = WalkTheWay(move, null, Copy(map), items - doorsFound.Count, ref newDoors, keysFound.ToList());
                            doorsFound.AddRange(newDoors);
                        }
                        else
                        {
                            var newDoors = new List<char>();
                            path = WalkTheWay(move, current, Copy(map), items - doorsFound.Count, ref newDoors, keysFound.ToList());
                            doorsFound.AddRange(newDoors);
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