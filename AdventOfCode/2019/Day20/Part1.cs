using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdventOfCode.Utils;

namespace AdventOfCode2019.Day20
{
    public class Part1
    {
        private static bool debug = false;
        public void Run()
        {
            var input = File.ReadAllLines("2019/Day20/Input.txt");
            Coordinate start;
            Coordinate goal;
            var donut = GetDonut(input);
            var portals = GetPortals(input, out start, out goal);

            var res = CalculateShortestPath(start, goal, donut, portals);
            Console.WriteLine(res);
        }

        public char[][] GetDonut(string[] input)
        {
            return input.Select(i => i.Select(c => char.IsLetter(c) ? ' ' : c).ToArray()).ToArray();
        }

        public void PrintDonut(char[][] donut, Coordinate c = null)
        {
            Console.SetCursorPosition(0, 0);
            foreach (var arr in donut)
            {
                Console.WriteLine(string.Join("", arr));
            }

            if (c != null)
            {
                Console.SetCursorPosition(c.X, c.Y);
                Console.Write('o');
            }
        }

        private List<Portal> GetPortals(string[] input, out Coordinate start, out Coordinate goal)
        {
            var portals = new List<Portal>();
            var halfPortals = new Dictionary<string, Coordinate>();

            for (int y = 0; y < input.Length - 1; y++)
            {
                for (int x = 0; x < input[y].Length - 1; x++)
                {
                    if (char.IsUpper(input[y][x])) //Portal found!
                    {
                        if (char.IsUpper(input[y + 1][x]))
                        {
                            var portalName = $"{input[y][x]}{input[y + 1][x]}";
                            Coordinate loc;
                            if (y + 2 < input.Length && input[y + 2][x] == '.')
                                loc = new Coordinate(x, y + 2);
                            else
                                loc = new Coordinate(x, y - 1);

                            if (halfPortals.ContainsKey(portalName))
                            {
                                portals.Add(new Portal { Name = portalName, Location1 = halfPortals[portalName], Location2 = loc });
                                halfPortals.Remove(portalName);
                            }
                            else
                            {
                                halfPortals.Add(portalName, loc);
                            }
                        }
                        else if (char.IsUpper(input[y][x + 1]))
                        {
                            var portalName = $"{input[y][x]}{input[y][x + 1]}";
                            Coordinate loc;
                            if (x + 2 < input[y].Length && input[y][x + 2] == '.')
                                loc = new Coordinate(x + 2, y);
                            else
                                loc = new Coordinate(x - 1, y);
                            if (halfPortals.ContainsKey(portalName))
                            {
                                portals.Add(new Portal { Name = portalName, Location1 = halfPortals[portalName], Location2 = loc });
                                halfPortals.Remove(portalName);
                            }
                            else
                            {
                                halfPortals.Add(portalName, loc);
                            }
                        }
                    }
                }
            }

            start = halfPortals["AA"];
            goal = halfPortals["ZZ"];

            return portals;
        }



        private int CalculateShortestPath(Coordinate start, Coordinate goal, char[][] donut, List<Portal> portals)
        {
            var current = start;
            var moves = 0;
            while (true)
            {
                if (current.Equals(goal))
                    return moves;

                moves++;

                if (debug)
                {
                    PrintDonut(donut, current);
                    Thread.Sleep(50);
                }

                var possiblePortal = portals.FirstOrDefault(p => p.Location1.Equals(current));
                if (possiblePortal != null)
                {
                    current = possiblePortal.Location2;
                    moves++;
                }
                else
                {
                    possiblePortal = portals.FirstOrDefault(p => p.Location2.Equals(current));
                    if (possiblePortal != null)
                    {
                        current = possiblePortal.Location1;
                        moves++;
                    }
                }

                donut[current.Y][current.X] = ',';
                var possibleMoves = GetPossibleMoves(current, donut);

                if (possibleMoves.Count == 1)
                {
                    current = possibleMoves.Single();
                }
                else
                {
                    var bestMove = 100000;
                    foreach (var move in possibleMoves)
                    {
                        var newMoves = CalculateShortestPath(move, goal, Copy(donut), portals);
                        if (newMoves < bestMove)
                            bestMove = newMoves;
                    }

                    return bestMove + moves;
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


        private bool CanMove(char c)
        {
            return c == '.';
        }
        private char[][] Copy(char[][] map)
        {
            return map.Select(m => m.ToArray()).ToArray();
        }

        private class Portal
        {
            public string Name { get; set; }
            public Coordinate Location1 { get; set; }
            public Coordinate Location2 { get; set; }
        }

        private class Path
        {
            public int Cost { get; set; }
            public Coordinate Start { get; set; }
            public Coordinate Goal { get; set; }
        }
    }
}