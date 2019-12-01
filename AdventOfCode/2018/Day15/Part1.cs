using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;

namespace AdventOfCode2018.Day15
{
    public class Part1
    {
        public static void Run()
        {
            var input = File.ReadAllLines("2018/Day15/Input.txt");
            var map = input.Select(line => line.ToArray()).ToArray();

            var elfs = new List<Player>();
            var goblins = new List<Player>();
            for (int y = 0; y < map.Length; y++)
            {
                for (int x = 0; x < map[0].Length; x++)
                {
                    if (map[y][x] == 'E')
                    {
                        elfs.Add(new Player { X = x, Y = y, Type = 0 });
                    }
                    else if (map[y][x] == 'G')
                    {
                        goblins.Add(new Player { X = x, Y = y, Type = 1 });
                    }
                }
            }


            //DebugPrint(map);
            var rounds = 0;
            while (elfs.Count != 0 && goblins.Count != 0)
            {
                var playingOrder = elfs.Concat(goblins).OrderBy(p => p.Y).ThenBy(p => p.X).ToList();
                foreach (var player in playingOrder)
                {
                    if (!player.IsDead)
                    {
                        var opponents = player.Type == 0 ? goblins : elfs;
                        var playersToAttack = new List<Player>();

                        var opponentsInRangeAtStart = opponents.Where(o => IsAdjacent(player.X, player.Y, o));
                        if (opponentsInRangeAtStart.Any())
                        {
                            playersToAttack = opponentsInRangeAtStart.ToList();
                        }
                        else
                        {
                            var moving = true;
                            var movingCost = GetEmptyMatrix(map.Length, map[0].Length);
                            
                            movingCost[player.Y][player.X] = 0;
                            var newMoves = new List<Move>() { new Move { X = player.X, Y = player.Y } };

                            while (moving)
                            {
                                newMoves = Advance(map, movingCost, newMoves);
                                if (!newMoves.Any())
                                    moving = false;

                                foreach (var move in newMoves.OrderBy(p => p.Y).ThenBy(p => p.X))
                                {
                                    var opponentsInRange = opponents.Where(o => IsAdjacent(move.X, move.Y, o));
                                    if (opponentsInRange.Any())
                                    {
                                        var moveToMove = move;
                                        if (moveToMove.IsFirst)
                                            playersToAttack = opponentsInRange.ToList();

                                        while (!moveToMove.IsFirst)
                                            moveToMove = moveToMove.Previous;

                                        DoMove(map, player, moveToMove);
                                        moving = false;
                                        break;
                                    }
                                }
                            }

                        }

                        var opponent = playersToAttack.OrderBy(p => p.HitPoints).ThenBy(p => p.Y).ThenBy(p => p.X).FirstOrDefault();
                        if (opponent != null)
                        {
                            if (opponent.HitPoints > 3)
                            {
                                opponent.HitPoints -= 3;
                            }
                            else
                            {
                                opponent.IsDead = true;
                                map[opponent.Y][opponent.X] = '.';

                                elfs.Remove(opponent);
                                goblins.Remove(opponent);
                            }
                        }
                    }
                }
                rounds++;
                //DebugPrint(map);
                //Console.WriteLine(rounds);
                //Console.WriteLine();

            }
            rounds--;
            Console.WriteLine($"Rounds: {rounds}. Goblins {goblins.Count} ({goblins.Sum(g => g.HitPoints)}). Elfs {elfs.Count} ({elfs.Sum(e => e.HitPoints)})");
            Console.WriteLine(rounds * goblins.Sum(g => g.HitPoints) + rounds * elfs.Sum(e => e.HitPoints)); //260508 to high
        }

        private static List<Move> Advance(char[][] map, int[][] movingCost, List<Move> coordinatesToMove)
        {
            var newList = new List<Move>();
            foreach (var coordinate in coordinatesToMove)
            {
                if (coordinate.Y > 1 && map[coordinate.Y - 1][coordinate.X] == '.' && movingCost[coordinate.Y - 1][coordinate.X] == -1) //Up
                {
                    movingCost[coordinate.Y - 1][coordinate.X] = movingCost[coordinate.Y][coordinate.X] + 1;
                    newList.Add(new Move { Previous = coordinate, X = coordinate.X, Y = coordinate.Y - 1 });
                }

                if (coordinate.X > 1 && map[coordinate.Y][coordinate.X - 1] == '.' && movingCost[coordinate.Y][coordinate.X - 1] == -1) //Left
                {
                    movingCost[coordinate.Y][coordinate.X - 1] = movingCost[coordinate.Y][coordinate.X] + 1;
                    newList.Add(new Move { Previous = coordinate, X = coordinate.X - 1, Y = coordinate.Y });
                }

                if (coordinate.X < map.Length + 1 && map[coordinate.Y][coordinate.X + 1] == '.' && movingCost[coordinate.Y][coordinate.X + 1] == -1) //Right
                {
                    movingCost[coordinate.Y][coordinate.X + 1] = movingCost[coordinate.Y][coordinate.X] + 1;
                    newList.Add(new Move { Previous = coordinate, X = coordinate.X + 1, Y = coordinate.Y });
                }

                if (coordinate.X < map[0].Length + 1 && map[coordinate.Y + 1][coordinate.X] == '.' && movingCost[coordinate.Y + 1][coordinate.X] == -1) //Down
                {
                    movingCost[coordinate.Y + 1][coordinate.X] = movingCost[coordinate.Y][coordinate.X] + 1;
                    newList.Add(new Move { Previous = coordinate, X = coordinate.X, Y = coordinate.Y + 1 });
                }
            }

            return newList;
        }

        private static int[][] GetEmptyMatrix(int height, int width)
        {
            var arr = new int[height][];
            for (int i = 0; i < height; i++)
            {
                arr[i] = Enumerable.Repeat(-1, width).ToArray();
            }

            return arr;
        }


        private static bool IsAdjacent(int x, int y, Player p)
        {
            return x == p.X && y == p.Y + 1
                || x == p.X && y == p.Y - 1
                || x == p.X + 1 && y == p.Y
                || x == p.X - 1 && y == p.Y;
        }

        private static void DoMove(char[][] map, Player player, Move move)
        {
            map[player.Y][player.X] = '.';
            map[move.Y][move.X] = player.Type == 0 ? 'E' : 'G';
            player.X = move.X;
            player.Y = move.Y;
        }

        private static void DebugPrint(char[][] map)
        {
            var str = new StringBuilder();
            foreach (var line in map)
            {
                str.AppendLine(string.Join("", line));
            }
            Console.WriteLine(str);
        }

        private class Player
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int HitPoints { get; set; } = 200;
            public int Type { get; set; }

            public bool IsDead { get; set; } = false;
        }

        private class Move
        {
            public Move Previous { get; set; }
            public int X { get; set; }
            public int Y { get; set; }

            public bool IsFirst => Previous.Previous == null;
        }
    }
}
