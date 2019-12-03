using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Drawing;

namespace AdventOfCode2019.Day3
{
    public class Instruction
    {
        public char Direction { get; set; }
        public int Length { get; set; }
    }
    public class Part1
    {
        public static void Run()
        {
            var input = File.ReadAllLines("2019/Day3/Input.txt");
            var instructions = ParseInput(input);

            var board = CreateBoard(instructions, out Point centralPort);

            foreach (var instructionSet in instructions)
            {
                UpdateBoard(board, centralPort, instructionSet);
            }

            foreach (var row in board)
            {
                Console.WriteLine(new String(row.ToArray()));
            }

            var distance = int.MaxValue;
            for (int i = 0; i < board.Length; i++)
            {
                for (int j = 0; j < board[0].Length; j++)
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

        private static List<List<Instruction>> ParseInput(string[] input)
        {
            var result = new List<List<Instruction>>();
            foreach (var path in input)
            {
                var list = new List<Instruction>();
                foreach (var instruction in path.Split(','))
                {
                    list.Add(new Instruction
                    {
                        Direction = instruction.First(),
                        Length = int.Parse(new String(instruction.Skip(1).ToArray()))
                    });
                }
                result.Add(list);
            }
            return result;
        }

        private static char[][] CreateBoard(List<List<Instruction>> instructions, out Point centralPort)
        {
            var minX = 0;
            var maxX = 0;
            var minY = 0;
            var maxY = 0;

            foreach (var instructionSet in instructions)
            {
                var x = -minX;
                var y = -minY;

                foreach (var instruction in instructionSet)
                {
                    if (instruction.Direction == 'U')
                        y += instruction.Length;
                    else if (instruction.Direction == 'D')
                        y -= instruction.Length;
                    else if (instruction.Direction == 'L')
                        x -= instruction.Length;
                    else if (instruction.Direction == 'R')
                        x += instruction.Length;

                    maxX = Math.Max(maxX, x);
                    minX = Math.Min(minX, x);
                    maxY = Math.Max(maxY, y);
                    minY = Math.Min(minY, y);
                }
            }

            var board = new char[maxY - minY][];
            for (int i = 0; i < board.Length; i++)
            {
                board[i] = Enumerable.Repeat(' ', maxX - minX + 1).ToArray();
            }
            centralPort = new Point(-minX, -minY);
            board[centralPort.Y][centralPort.X] = 'o';

            return board;
        }

        private static void UpdateBoard(char[][] board, Point currentPoint, List<Instruction> instructions)
        {
            var first = true;
            foreach (var instruction in instructions)
            {
                //foreach (var row in board)
                //{
                //    Console.WriteLine(new String(row.ToArray()));
                //}
                //Console.WriteLine("======");

                if (!first)
                    board[currentPoint.Y][currentPoint.X] = '+';
                first = false;

                for (int i = 1; i <= instruction.Length; i++)
                {
                    if (instruction.Direction == 'R')
                    {
                        currentPoint.X++;
                        if (board[currentPoint.Y][currentPoint.X] == ' ')
                            board[currentPoint.Y][currentPoint.X] = '-';
                        else
                            board[currentPoint.Y][currentPoint.X] = 'X';
                    }
                    else if (instruction.Direction == 'L')
                    {
                        currentPoint.X--;
                        if (board[currentPoint.Y][currentPoint.X] == ' ')
                            board[currentPoint.Y][currentPoint.X] = '-';
                        else
                            board[currentPoint.Y][currentPoint.X] = 'X';
                    }
                    else if (instruction.Direction == 'U')
                    {
                        currentPoint.Y++;
                        if (board[currentPoint.Y][currentPoint.X] == ' ')
                            board[currentPoint.Y][currentPoint.X] = '|';
                        else
                            board[currentPoint.Y][currentPoint.X] = 'X';
                    }
                    else if (instruction.Direction == 'D')
                    {
                        currentPoint.Y--;
                        if (board[currentPoint.Y][currentPoint.X] == ' ')
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
                        return new Point(j, i);
                }
            }

            throw new ApplicationException("oops");
        }
    }
}
