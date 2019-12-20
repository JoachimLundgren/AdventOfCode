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
        private object syncRoot = new object();
        public int Depth { get; set; }
        public void Run()
        {
            var input = File.ReadAllLines("2019/Day18/Input.txt");
            var map = input.Select(line => line.ToCharArray()).ToArray();

            Hello(Copy(map));

            //var keys = map.Sum(line => line.Count(c => char.IsLower(c)));


            //Coordinate currentCoordinate = null;
            //for (int y = 0; y < input.Length; y++)
            //{
            //    var x = input[y].IndexOf('@');
            //    if (x != -1)
            //    {
            //        currentCoordinate = new Coordinate(x, y);
            //        map[y][x] = '.';
            //        break;
            //    }
            //}

            //var length = DoIt(currentCoordinate, Copy(map));
            //var length = WalkTheWay(currentCoordinate, Copy(map), keys);
            //Console.WriteLine(length);
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

        private void Hello(char[][] map)
        {
            var keys = new List<Key>();
            for (char i = 'a'; i <= 'z'; i++)
            {
                var key = FindAllKeys(i, Copy(map));
                if (key != null)
                {
                    keys.Add(key);
                    Console.Write(i);
                }
            }
            Console.WriteLine();
            Console.WriteLine(CalcPath(FindAllKeys('@', Copy(map)), keys));

        }

        private int CalcPath(Key current, List<Key> keys)
        {
            if (!keys.Any())
                return 0;

            var reachable = current.Paths.Where(p => !p.Value.BlockedBy.Any()).ToList();
            var best = 1000000;
            foreach (var path in reachable)
            {
                var next = keys.Single(k => k.C == path.Key);
                var newKeys = RemoveChar(path.Key, keys);
                var a = CalcPath(next, newKeys) + path.Value.Length;
                if (a < best)
                    best = a;
            }

            return best;
        }

        private List<Key> RemoveChar(char c, List<Key> keys)
        {
            var newKeys = keys
                .Where(k => k.C != c)
                .Select(k => new Key(k.C, k.Location, k.Paths.Where(kvp => kvp.Key != c).ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Clone()))).ToList();

            foreach (var k in newKeys)
            {
                foreach (var path in k.Paths.Values)
                    path.BlockedBy.Remove(c);
            }
            return newKeys;
        }

        private Key FindAllKeys(char c, char[][] map)
        {
            var coord = GetCoordinate(c, map);
            if (coord == null)
                return null;

            var keys = new Dictionary<char, Path>();
            FindKeys2(coord, map, 0, keys, new List<char>());
            keys.Remove(c);
            return new Key(c, coord, keys);
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
            public int Length { get; set; }
            public List<char> BlockedBy { get; set; }

            public Path Clone()
            {
                return new Path { Length = Length, BlockedBy = BlockedBy.ToList() };
            }
        }

        private int DoIt(Coordinate current, char[][] map)
        {
            if (!map.Any(m => m.Any(c => char.IsLower(c))))
                return 0; //No more keys

            //Depth++;
            //Console.SetCursorPosition(0, 4);
            //Console.WriteLine(Depth);
            var nextKeys = new Dictionary<char, int>();
            FindKeys(current, Copy(map), 0, nextKeys);

            var best = 100000;
            Parallel.ForEach(nextKeys, key =>
            {
                var coordinate = GetCoordinate(key.Key, map);
                var newMap = Copy(map);
                Unlock(key.Key, newMap);
                var res = DoIt(coordinate, newMap) + key.Value;
                lock (syncRoot)
                {
                    if (res < best)
                        best = res;
                }
            });
            //foreach (var key in nextKeys)
            //{
            //    var coordinate = GetCoordinate(key.Key, map);
            //    var newMap = Copy(map);
            //    Unlock(key.Key, newMap);
            //    var res = DoIt(coordinate, newMap) + key.Value;
            //    if (res < best)
            //        best = res;
            //}

            //Depth--;
            return best;
        }

        private void FindKeys2(Coordinate current, char[][] map, int moves, Dictionary<char, Path> keys, List<char> doors)
        {
            while (true)
            {
                var c = map[current.Y][current.X];

                if (char.IsLower(c))
                {
                    //Key found!
                    if (keys.ContainsKey(c))
                    {
                        if (keys[c].Length > moves)
                        {
                            keys[c].Length = moves;
                            keys[c].BlockedBy = doors.ToList();
                        }
                        else
                            return;
                    }
                    else
                        keys.Add(c, new Path { Length = moves, BlockedBy = doors.ToList() });
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
                        FindKeys2(move, Copy(map), moves, keys, doors.ToList());
                    }
                    return;
                }
            }
        }

        private void FindKeys(Coordinate current, char[][] map, int moves, Dictionary<char, int> keys)
        {
            while (true)
            {
                var c = map[current.Y][current.X];

                if (char.IsLower(c))
                {
                    if (keys.ContainsKey(c))
                    {
                        if (keys[c] > moves)
                            keys[c] = moves;
                        else
                            return;
                    }
                    else
                        keys.Add(c, moves);
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
                        FindKeys(move, Copy(map), moves, keys);
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


        private bool CanMove(char c)
        {
            return c == '.' || char.IsLetter(c) || c == '@';
        }
    }
}