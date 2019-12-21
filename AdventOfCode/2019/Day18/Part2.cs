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
                map[start.Y - 1][start.X + 1] = '3';
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

            Console.WriteLine();
            var keys = map.Sum(row => row.Count(c => char.IsLower(c)));
            var p = CalculateShortestPath(paths, keys);
            Console.WriteLine();
            Console.WriteLine(p.Length); //2188 to high
        }

        private RobotGroup CalculateShortestPath(Dictionary<char, List<Path>> paths, int keys)
        {
            var startGroup = new RobotGroup();

            startGroup.Robots.Add(1, '1');
            startGroup.Robots.Add(2, '2');
            startGroup.Robots.Add(3, '3');
            startGroup.Robots.Add(4, '4');

            var allGroups = new List<RobotGroup>() { startGroup };
            var mostKeys = 0;
            var duplicates = 0;
            while (true)
            {
                var shortestGroup = allGroups.MinBy(fp => fp.Length);
                if (keys == shortestGroup.KeysFound)
                    return shortestGroup;

                var nextMoves = GetPossiblePaths(shortestGroup, paths);

                if (shortestGroup.KeysFound > mostKeys)
                {
                    mostKeys = shortestGroup.KeysFound;
                    duplicates += RemoveDuplicates(allGroups); //Slow af, try doing it fewer times?
                    Console.WriteLine($"Found {mostKeys} of {keys} - Groups {allGroups.Count} Duplicates {duplicates}");
                }
                allGroups.Remove(shortestGroup);
                foreach (var move in nextMoves)
                {
                    var clone = shortestGroup.Clone();
                    clone.Keys.Add(move.Key);
                    clone.Length += move.Cost;
                    clone.Robots[move.Robot] = move.Key;
                    allGroups.Add(clone);
                }
            }
        }

        private int RemoveDuplicates(List<RobotGroup> allGroups)
        {
            var duplicates = 0;
            foreach (var group in allGroups.ToList())
            {
                var existingGroup = allGroups.Any(g =>
                    g != group &&
                    g.Robots[1] == group.Robots[1] &&
                    g.Robots[2] == group.Robots[2] &&
                    g.Robots[3] == group.Robots[3] &&
                    g.Robots[4] == group.Robots[4] &&
                    g.Length <= group.Length &&
                    group.Keys.Intersect(g.Keys).Count() == group.Keys.Count());

                if (existingGroup)
                {
                    duplicates++;
                    allGroups.Remove(group);
                }
            }

            return duplicates;
        }

        private List<Collected> GetPossiblePaths(RobotGroup group, Dictionary<char, List<Path>> paths)
        {
            var result = new List<Collected>();

            foreach (var robot in group.Robots)
            {
                var possiblePaths = paths.GetValueOrDefault(robot.Value) ?? new List<Path>();

                foreach (var path in possiblePaths)
                {
                    if (!group.Keys.Contains(path.End))
                    {
                        if (path.BlockedBy.All(c => group.Keys.Contains(c)))
                        {
                            result.Add(new Collected(robot.Key, path.End, path.Cost));
                        }
                    }
                }
            }

            return result;
        }

        #region Pathfinder
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

        #endregion

        private class RobotGroup
        {
            public Dictionary<int, char> Robots { get; set; }
            public List<char> Keys { get; set; }
            public int Length { get; set; }
            public int KeysFound => Keys.Count;

            public RobotGroup()
            {
                Robots = new Dictionary<int, char>();
                Keys = new List<char>();
            }

            public RobotGroup Clone()
            {
                return new RobotGroup { Robots = Robots.ToDictionary(r => r.Key, r => r.Value), Keys = Keys.ToList(), Length = Length };
            }
        }

        private class Robot
        {
            public int Id { get; set; }
            public char Location { get; set; }

            public Robot Clone()
            {
                return new Robot { Id = Id, Location = Location };
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

        private class Collected
        {
            public int Robot { get; }
            public char Key { get; }
            public int Cost { get; }

            public Collected(int robot, char key, int cost)
            {
                Robot = robot;
                Key = key;
                Cost = cost;
            }

            public Collected Clone()
            {
                return new Collected(Robot, Key, Cost);
            }

            public override string ToString()
            {
                return $"{Robot} + {Key} == {Cost}";
            }
        }
    }
}