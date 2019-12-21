using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Utils;
using System.Threading;

namespace AdventOfCode2019.Day20
{
    public class Part2
    {
        private static bool debug = false;


        private char[][] originalDonut;
        private char[][] newDonut => Copy(originalDonut);
        public void Run()
        {
            var input = File.ReadAllLines("2019/Day20/Input.txt");
            Coordinate start;
            Coordinate goal;
            originalDonut = GetDonut(input);
            var portals = GetPortals(input, out start, out goal);

            var paths = GetPaths(start, goal, portals);

            var res = GetShortestPath(start, goal, portals, paths);
            Console.WriteLine(res); //5356 too low
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
                                loc = new Coordinate(portalName, x, y + 2);
                            else
                                loc = new Coordinate(portalName, x, y - 1);

                            if (halfPortals.ContainsKey(portalName))
                            {
                                var isOuter = y == 0 || y + 2 == input.Length;
                                portals.Add(new Portal
                                {
                                    Name = portalName,
                                    LargeLocation = isOuter ? loc : halfPortals[portalName],
                                    SmallLocation = isOuter ? halfPortals[portalName] : loc
                                });
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
                                loc = new Coordinate(portalName, x + 2, y);
                            else
                                loc = new Coordinate(portalName, x - 1, y);


                            if (halfPortals.ContainsKey(portalName))
                            {
                                var isOuter = x == 0 || x + 2 == input[y].Length;
                                portals.Add(new Portal
                                {
                                    Name = portalName,
                                    LargeLocation = isOuter ? loc : halfPortals[portalName],
                                    SmallLocation = isOuter ? halfPortals[portalName] : loc
                                });
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

        private Dictionary<Coordinate, List<Path>> GetPaths(Coordinate start, Coordinate goal, List<Portal> portals)
        {
            var paths = new Dictionary<Coordinate, List<Path>>();

            var portalCopy = portals.ToList();
            portalCopy.Add(new Portal() { LargeLocation = goal, Name = "ZZ" });

            var portalPaths = new List<Path>();
            GetPaths(start, portalCopy, newDonut, portalPaths);
            portalPaths.ForEach(p => p.Start = start);
            paths.Add(start, portalPaths);

            foreach (var portal in portals)
            {
                portalPaths = new List<Path>();
                GetPaths(portal.LargeLocation, portalCopy, newDonut, portalPaths);
                portalPaths.ForEach(p => p.Start = portal.LargeLocation);
                paths.Add(portal.LargeLocation, portalPaths);

                portalPaths = new List<Path>();
                GetPaths(portal.SmallLocation, portalCopy, newDonut, portalPaths);
                portalPaths.ForEach(p => p.Start = portal.LargeLocation);
                paths.Add(portal.SmallLocation, portalPaths);
            }

            return paths;
        }

        private void GetPaths(Coordinate start, List<Portal> portals, char[][] donut, List<Path> paths, int moves = 0)
        {
            var current = start;

            while (true)
            {
                var portal = portals.SingleOrDefault(p => current.Equals(p.LargeLocation) || current.Equals(p.SmallLocation));
                if (portal != null && moves > 0)
                    paths.Add(new Path { Goal = current, Cost = moves });

                moves++;
                donut[current.Y][current.X] = ',';
                var possibleMoves = GetPossibleMoves(current, donut);

                if (possibleMoves.Count == 1)
                {
                    current = possibleMoves.Single();
                }
                else
                {
                    foreach (var move in possibleMoves)
                    {
                        GetPaths(move, portals, Copy(donut), paths, moves);
                    }

                    return;
                }
            }
        }



        private int GetShortestPath(Coordinate start, Coordinate goal, List<Portal> portals, Dictionary<Coordinate, List<Path>> paths)
        {
            var alternatives = new List<Alternative>();
            alternatives.Add(new Alternative() { Cost = 0, Current = start, Level = 0, Log = new List<string>() });

            while (true)
            {
                var best = alternatives.MinBy(a => a.Cost);

                if (best.Current.Equals(goal) && best.Level == 0)
                {
                    if (debug)
                        best.Log.ForEach(l => Console.WriteLine(l));
                    return best.Cost;
                }

                alternatives.Remove(best);
                foreach (var path in paths[best.Current])
                {
                    var clone = best.Clone(path);
                    if (clone.Current.Equals(goal))
                    {
                        if (clone.Level == 0)
                        {
                            alternatives.Add(clone);
                        }
                    }
                    else
                    {
                        UsePortal(clone, portals);
                        if (clone.Level >= 0)
                            alternatives.Add(clone);
                    }
                }
            }
        }

        private void UsePortal(Alternative alt, List<Portal> portals)
        {
            var portal = portals.Single(p => p.LargeLocation.Equals(alt.Current) || p.SmallLocation.Equals(alt.Current));
            if (portal.LargeLocation.Equals(alt.Current))
            {
                alt.Level--;
                if (debug) alt.Log.Add($"Recurse into level {alt.Level} through {alt.Current} (1 step)");
                alt.Current = portal.SmallLocation;
                alt.Cost++;
            }
            else
            {
                alt.Level++;
                if (debug) alt.Log.Add($"Return to level {alt.Level} through {alt.Current} (1 step)");
                alt.Current = portal.LargeLocation;
                alt.Cost++;
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
        private Dictionary<int, char[][]> Copy(Dictionary<int, char[][]> maps)
        {
            return maps.ToDictionary(m => m.Key, m => Copy(m.Value));
        }

        private char[][] Copy(char[][] map)
        {
            return map.Select(m => m.ToArray()).ToArray();
        }

        private class Portal
        {
            public string Name { get; set; }
            public Coordinate LargeLocation { get; set; }
            public Coordinate SmallLocation { get; set; }
        }

        private class Path
        {
            public int Cost { get; set; }
            public Coordinate Start { get; set; }
            public Coordinate Goal { get; set; }
        }

        private class Alternative
        {
            public int Cost { get; set; }
            public int Level { get; set; }
            public Coordinate Current { get; set; }
            public List<string> Log { get; set; }
            public Alternative Clone(Path p)
            {
                var alt = new Alternative() { Cost = Cost + p.Cost, Current = p.Goal, Level = Level, Log = Log.ToList() };
                if (debug) alt.Log.Add($"Walk from {p.Start} to {p.Goal} ({p.Cost} steps)");
                return alt;
            }
        }
    }
}