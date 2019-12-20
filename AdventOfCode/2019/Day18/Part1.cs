using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Utils;
using System.Threading.Tasks;

namespace AdventOfCode2019.Day18
{
    public class Part1
    {
        public void Run()
        {
            var input = File.ReadAllLines("2019/Day18/Input.txt");
            var map = input.Select(line => line.ToCharArray()).ToArray();

            Hello(Copy(map));
        }

        private void Hello(char[][] map)
        {
            var paths = new Dictionary<char, List<Path>>();
            paths.Add('@', FindAllPaths('@', Copy(map)));
            for (char i = 'a'; i <= 'z'; i++)
            {
                var path = FindAllPaths(i, Copy(map));
                if (path.Any())
                {
                    paths.Add(i, path);
                    Console.Write(i);
                }
            }

            var p = CalculateShortestPath(paths);
            Console.WriteLine();
            Console.WriteLine(string.Join(", ", p.Path));
            Console.WriteLine(p.Length); //6362 wrong
        }

        private PathObj CalculateShortestPath(Dictionary<char, List<Path>> paths)
        {
            var keys = paths.Count;
            var finalPaths = paths['@'].Where(p => !p.BlockedBy.Any()).Select(p => new PathObj(p.Cost, p.Start, p.End)).ToList();

            while (true)
            {
                var shortestPath = finalPaths.OrderBy(fp => fp.Length).First(); //TODO keep sorted???
                if (keys == shortestPath.Path.Count)
                    return shortestPath;


                var nextMoves = GetPossiblePaths(shortestPath, paths[shortestPath.Path.Last()]);

                finalPaths.Remove(shortestPath);
                foreach (var move in nextMoves)
                {
                    var existingPath = finalPaths.FirstOrDefault(fp =>
                        fp.Path.Last() == move.End && shortestPath.Path.All(c => fp.Path.Contains(c)));

                    if (existingPath == null || existingPath.Length > shortestPath.Length + move.Cost)
                    {
                        var clone = shortestPath.Clone();
                        clone.Path.Add(move.End);
                        clone.Length += move.Cost;
                        finalPaths.Add(clone);
                    }
                }
            }
        }

        private List<Path> GetPossiblePaths(PathObj p, List<Path> paths)
        {
            return paths.Where(path => path.BlockedBy.All(blocked => p.Path.Contains(blocked)) && !p.Path.Contains(path.End)).OrderBy(x => x.Cost).ToList();
        }

        private List<Path> FindAllPaths(char c, char[][] map)
        {
            var result = new List<Path>();
            var coord = GetCoordinate(c, map);

            if (coord != null)
                FindKeys(coord, c, map, 0, result, new List<char>());

            return result.Where(r => r.Cost > 0).ToList();
        }

        private void FindKeys(Coordinate current, char start, char[][] map, int moves, List<Path> paths, List<char> doors)
        {
            while (true)
            {
                var c = map[current.Y][current.X];

                if (char.IsLower(c))
                {
                    //Key found!
                    var path = paths.FirstOrDefault(p => p.End == c);
                    if (path != null)
                    {
                        if (path.Cost > moves)
                        {
                            path.Cost = moves;
                            path.BlockedBy = doors.ToList();
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        paths.Add(new Path { Start = start, End = c, Cost = moves, BlockedBy = doors.ToList() });
                    }
                }
                else if (char.IsUpper(c))
                {
                    doors.Add(char.ToLower(c));
                    map[current.Y][current.X] = ',';
                }
                else
                {
                    map[current.Y][current.X] = ',';
                }


                var possibleMoves = GetPossibleMoves(current, map);
                if (!possibleMoves.Any())
                    return;

                moves++;
                if (possibleMoves.Count == 1)
                {
                    current = possibleMoves.Single();
                }
                else
                {
                    foreach (var move in possibleMoves)
                    {
                        FindKeys(move, start, Copy(map), moves, paths, doors.ToList());
                    }
                    return;
                }
            }
        }

        private Coordinate GetCoordinate(char c, char[][] map)
        {
            for (int y = 0; y < map.Length; y++)
            {
                for (int x = 0; x < map[y].Length; x++)
                {
                    if (map[y][x] == c)
                        return new Coordinate(x, y);
                }
            }

            return null;
        }


        private char[][] Copy(char[][] map)
        {
            return map.Select(m => m.ToArray()).ToArray();
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
            return c == '.' || char.IsLetter(c) || c == '@';
        }

        private class PathObj
        {
            public List<char> Path { get; set; }
            public int Length { get; set; }

            public PathObj(int cost, params char[] path)
            {
                Length = cost;
                Path = path.ToList();
            }

            public PathObj Clone()
            {
                return new PathObj(Length, Path.ToArray());
            }
        }

        private class Key
        {
            public char C { get; }
            public Coordinate Location { get; }
            public Dictionary<char, Path> Paths { get; }

            public Key(char c, Coordinate loc, Dictionary<char, Path> paths)
            {
                C = c;
                Location = loc;
                Paths = paths;

                foreach (var p in Paths)
                {
                    p.Value.BlockedBy.Remove(c);
                }
            }
        }

        private class Path
        {
            public char Start { get; set; }
            public char End { get; set; }
            public int Cost { get; set; }
            public List<char> BlockedBy { get; set; }
        }
    }
}