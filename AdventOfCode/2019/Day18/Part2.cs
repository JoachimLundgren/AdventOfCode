using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdventOfCode.Utils;

namespace AdventOfCode2019.Day18
{
    public class Part2
    {
        public void Run()
        {
            var input = File.ReadAllLines("2019/Day18/Input.txt");
            var map = input.Select(line => line.ToCharArray()).ToArray();

            UpdateMap(map);
            Hello(Copy(map));
        }

        private void UpdateMap(char[][] map)
        {
            var robots = map.Sum(row => row.Count(c => c == '@'));
            if (robots == 1)
            {
                var start = GetCoordinate('@', map);
                map[start.Y][start.X - 1] = '#';
                map[start.Y][start.Y + 1] = '#';
                map[start.Y][start.X] = '#';
                map[start.Y - 1][start.X] = '#';
                map[start.Y + 1][start.X] = '#';
                map[start.Y + 1][start.X + 1] = '1';
                map[start.Y + 1][start.X - 1] = '2';
                map[start.Y - 1][start.X - 1] = '3';
                map[start.Y - 1][start.X - 1] = '4';
            }
            else if (robots == 4)
            {
                var start = GetCoordinate('@', map);
                map[start.Y][start.X] = '1';
                start = GetCoordinate('@', map);
                map[start.Y][start.X] = '2';
                start = GetCoordinate('@', map);
                map[start.Y][start.X] = '3';
                start = GetCoordinate('@', map);
                map[start.Y][start.X] = '4';
            }
            else
            {
                throw new ApplicationException("Fuck it");
            }
        }

        private void Hello(char[][] map)
        {
            var paths = new Dictionary<char, List<Path>>();
            paths.Add('1', FindAllPaths('1', Copy(map)));
            paths.Add('2', FindAllPaths('2', Copy(map)));
            paths.Add('3', FindAllPaths('3', Copy(map)));
            paths.Add('4', FindAllPaths('4', Copy(map)));
            for (char i = 'a'; i <= 'z'; i++)
            {
                var path = FindAllPaths(i, Copy(map));
                if (path.Any())
                {
                    paths.Add(i, path);
                    Console.Write(i);
                }
            }

            var keys = map.Sum(row => row.Count(c => char.IsLower(c)));
            var p = CalculateShortestPath(paths, keys);
            Console.WriteLine();
            //Console.WriteLine(string.Join(", ", p.));
            Console.WriteLine(p.Length);
        }

        private RobotGroup CalculateShortestPath(Dictionary<char, List<Path>> paths, int keys)
        {
            //var keys = paths.Count - 4;
            //var robot1Paths = paths['1'].Where(p => !p.BlockedBy.Any()).Select(p => new RobotPath(p.Cost, p.Start, p.End)).ToList();
            //var robot2Paths = paths['2'].Where(p => !p.BlockedBy.Any()).Select(p => new RobotPath(p.Cost, p.Start, p.End)).ToList();
            //var robot3Paths = paths['3'].Where(p => !p.BlockedBy.Any()).Select(p => new RobotPath(p.Cost, p.Start, p.End)).ToList();
            //var robot4Paths = paths['4'].Where(p => !p.BlockedBy.Any()).Select(p => new RobotPath(p.Cost, p.Start, p.End)).ToList();

            var startGroup = new RobotGroup() { Robots = new Dictionary<int, RobotPath>() };

            startGroup.Robots.Add(1, new RobotPath(0, '1'));
            startGroup.Robots.Add(2, new RobotPath(0, '2'));
            startGroup.Robots.Add(3, new RobotPath(0, '3'));
            startGroup.Robots.Add(4, new RobotPath(0, '4'));
            var allGroups = new List<RobotGroup>() { startGroup };

            while (true)
            {
                var shortestGroup = allGroups.OrderBy(fp => fp.Length).First(); //TODO keep sorted???
                if (keys == shortestGroup.KeysFound)
                    return shortestGroup;

                var nextMoves = GetPossiblePaths(shortestGroup, paths);

                allGroups.Remove(shortestGroup);
                foreach (var move in nextMoves)
                {
                    /*var existingPath = allGroups.FirstOrDefault(fp =>
                        fp.Path.Last() == move.End && shortestGroup.Path.All(c => fp.Path.Contains(c)));

                    if (existingPath == null || existingPath.Length > shortestGroup.Length + move.Cost)
                    {
                        var clone = shortestGroup.Clone();
                        clone.Path.Add(move.End);
                        clone.Length += move.Cost;
                        allGroups.Add(clone);
                    }*/

                    var clone = shortestGroup.Clone();
                    clone.Robots[move.Item1].Path.Add(move.Item2.End);
                    clone.Robots[move.Item1].Length += move.Item2.Cost;
                    allGroups.Add(clone);
                }
            }
        }

        private List<Tuple<int, Path>> GetPossiblePaths(RobotGroup group, Dictionary<char, List<Path>> paths)
        {
            var result = new List<Tuple<int, Path>>();

            var collectedKeys = group.Robots.Values.SelectMany(r => r.Path).Distinct().ToList();

            foreach (var robot in group.Robots)
            {
                var possiblePaths = paths.GetValueOrDefault(robot.Value.Path.Last()) ?? new List<Path>();

                foreach (var path in possiblePaths)
                {
                    if (path.BlockedBy.All(c => collectedKeys.Contains(c)))
                    {
                        result.Add(new Tuple<int, Path>(robot.Key, path));
                    }
                }
            }

            return result;
        }

        private List<Path> GetPossiblePaths(RobotPath p, List<Path> paths)
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
            return c == '.' || char.IsLetter(c) || char.IsDigit(c);
        }

        private class RobotGroup
        {
            public Dictionary<int, RobotPath> Robots { get; set; }
            public int Length => Robots.Values.Sum(r => r.Length);
            public int KeysFound => Robots.Values.Sum(r => r.Path.Count) - 4;

            public RobotGroup Clone()
            {
                return new RobotGroup { Robots = Robots.ToDictionary(r => r.Key, r => r.Value.Clone()) };
            }
        }

        private class RobotPath
        {
            public List<char> Path { get; set; }
            public int Length { get; set; }

            public RobotPath(int cost, params char[] path)
            {
                Length = cost;
                Path = path.ToList();
            }

            public RobotPath Clone()
            {
                return new RobotPath(Length, Path.ToArray());
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