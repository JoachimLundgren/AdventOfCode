using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Drawing;

namespace AdventOfCode2019.Day3
{
    public class Part1
    {
        public static void Run()
        {
            var input = File.ReadAllLines("2019/Day3/Input.txt");

            var board = new List<List<char>>();
            board.Add(new List<char>() { 'o' });
            var centralPort = new Point(0, 0);

            foreach (var line in input)
            {
                UpdateBoard(board, centralPort, line.Split(','));
                centralPort = FindCentralPort(board);
            }

            foreach (var row in board)
            {
                Console.WriteLine(new String(row.ToArray()));
            }

            var distance = int.MaxValue;
            for (int i = 0; i < board.Count; i++)
            {
                for (int j = 0; j < board.First().Count; j++)
                {
                    if (board[i][j] == 'X')
                    {
                        var newDistance = Math.Abs(centralPort.Y - i) + Math.Abs(centralPort.X - j);
                        distance = Math.Min(distance, newDistance);
                    }
                }
            }

            Console.WriteLine(distance);
        }


        private static void UpdateBoard(List<List<char>> board, Point currentPoint, string[] path)
        {
            var first = true;
            foreach (var instruction in path)
            {
                foreach (var row in board)
                {
                    Console.WriteLine(new String(row.ToArray()));
                }
                Console.WriteLine("======");

                var inst = instruction.First();
                var length = int.Parse(new String(instruction.Skip(1).ToArray()));
                if (inst == 'R')
                {
                    while (board.First().Count < currentPoint.X + length + 1)
                    {
                        foreach (var row in board)
                            row.Add('.');
                    }

                    if (!first)
                        board[currentPoint.Y][currentPoint.X] = '+';
                    first = false;
                    for (int i = 1; i <= length; i++)
                    {
                        currentPoint.X++;
                        if (board[currentPoint.Y][currentPoint.X] == '.')
                            board[currentPoint.Y][currentPoint.X] = '-';
                        else
                            board[currentPoint.Y][currentPoint.X] = 'X';
                    }

                }
                else if (inst == 'L')
                {
                    while (0 > currentPoint.X - length - 1)
                    {
                        foreach (var row in board)
                            row.Insert(0, '.');

                        currentPoint.X++;
                    }


                    if (!first)
                        board[currentPoint.Y][currentPoint.X] = '+';
                    first = false;

                    for (int i = 1; i <= length; i++)
                    {
                        currentPoint.X--;
                        if (board[currentPoint.Y][currentPoint.X] == '.')
                            board[currentPoint.Y][currentPoint.X] = '-';
                        else
                            board[currentPoint.Y][currentPoint.X] = 'X';
                    }
                }
                else if (inst == 'U')
                {
                    while (board.Count < currentPoint.Y + length + 1)
                    {
                        board.Add(Enumerable.Repeat('.', board.First().Count).ToList());
                    }

                    if (!first)
                        board[currentPoint.Y][currentPoint.X] = '+';
                    first = false;

                    for (int i = 1; i <= length; i++)
                    {
                        currentPoint.Y++;
                        if (board[currentPoint.Y][currentPoint.X] == '.')
                            board[currentPoint.Y][currentPoint.X] = '|';
                        else
                            board[currentPoint.Y][currentPoint.X] = 'X';
                    }
                }
                else if (inst == 'D')
                {
                    while (0 > currentPoint.Y - length - 1)
                    {
                        board.Insert(0, Enumerable.Repeat('.', board.First().Count).ToList());

                        currentPoint.Y++;
                    }


                    if (!first)
                        board[currentPoint.Y][currentPoint.X] = '+';
                    first = false;

                    for (int i = 1; i <= length; i++)
                    {
                        currentPoint.Y--;
                        if (board[currentPoint.Y][currentPoint.X] == '.')
                            board[currentPoint.Y][currentPoint.X] = '|';
                        else
                            board[currentPoint.Y][currentPoint.X] = 'X';
                    }
                }
            }
        }


        private static Point FindCentralPort(List<List<char>> board)
        {
            for (int i = 0; i < board.Count; i++)
            {
                for (int j = 0; j < board.First().Count; j++)
                {
                    if (board[i][j] == 'o')
                        return new Point(i, j);
                }
            }

            throw new ApplicationException("oops");
        }
    }
}
