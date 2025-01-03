﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdventOfCode.Utils;

namespace AdventOfCode2019.Day13
{
    public class Part2
    {
        public static int PaddleMove = 0;

        public static void Run()
        {
            var input = File.ReadAllLines("2019/Day13/Input.txt");

            var program = input.First().Split(',').Select(long.Parse).ToList();
            program[0] = 2; //free play
            var computer = new Computer { Program = program.ToList(), Inputs = new List<int>() { } };
            var grid = new Dictionary<Coordinate, int>();

            var score = 0;
            var scoreCoordinate = new Coordinate(-1, 0);
            var paddleCoordinate = new Coordinate(-1, -1);

            while (!computer.Finished)
            {
                var x = (int)computer.RunCode();
                var y = (int)computer.RunCode();
                var id = (int)computer.RunCode();
                var coordinate = new Coordinate(x, y);
                if (coordinate.Equals(scoreCoordinate))
                {
                    score = id;
                    PrintScore(score);
                }
                else
                {
                    if (!grid.ContainsKey(coordinate))
                        grid.Add(coordinate, id);
                    else
                        grid[coordinate] = id;

                    Console.SetCursorPosition(x + 17, y + 3);
                    Console.Write(GetTile(id));

                    if (id == 3) //Paddle moved
                    {
                        paddleCoordinate.X = x;
                        paddleCoordinate.Y = y;
                    }

                    if (id == 4) //Ball moved
                    {
                        if (x > paddleCoordinate.X)
                            PaddleMove = 1;
                        else if (x < paddleCoordinate.X)
                            PaddleMove = -1;
                        else
                            PaddleMove = 0;
                    }
                }
            }
            Console.SetCursorPosition(0, 4);
        }

        private static char GetTile(int id)
        {
            switch (id)
            {
                case 0: return ' '; //Empty
                case 1: return '#'; //wall
                case 2: return '.'; //Block
                case 3: return '_'; //Horizontal Paddle
                case 4: return 'o'; //ball

                default: throw new Exception("wtf");
            }
        }

        private static void PrintScore(int score)
        {
            Console.SetCursorPosition(32, 1);
            Console.WriteLine(score);
        }

        private class Computer
        {
            //private int inputPointer;

            public int Pointer { get; set; }
            public List<long> Program { get; set; }
            public bool Finished { get; set; }
            public List<int> Inputs { get; set; }
            public int RelativeBase { get; set; }

            public int NextInput
            {
                get
                {
                    return PaddleMove;
                }
            }

            public long RunCode()
            {
                var running = true;
                var outputValue = 0L;

                while (running)
                {
                    var op = Program[Pointer] % 100;
                    var a = (int)Program[Pointer] / 10000 % 10;
                    var b = (int)Program[Pointer] / 1000 % 10;
                    var c = (int)Program[Pointer] / 100 % 10;
                    if (op == 1)
                    {
                        SetValue(a, Pointer + 3, GetValue(c, Pointer + 1) + GetValue(b, Pointer + 2));
                        Pointer += 4;
                    }
                    else if (op == 2)
                    {
                        SetValue(a, Pointer + 3, GetValue(c, Pointer + 1) * GetValue(b, Pointer + 2));
                        Pointer += 4;
                    }
                    else if (op == 3)
                    {
                        SetValue(c, Pointer + 1, NextInput);
                        Pointer += 2;
                    }
                    else if (op == 4)
                    {
                        outputValue = GetValue(c, Pointer + 1);
                        Pointer += 2;
                        running = false;
                    }
                    else if (op == 5)
                    {
                        if (GetValue(c, Pointer + 1) != 0)
                            Pointer = (int)GetValue(b, Pointer + 2);
                        else
                            Pointer += 3;
                    }
                    else if (op == 6)
                    {
                        if (GetValue(c, Pointer + 1) == 0)
                            Pointer = (int)GetValue(b, Pointer + 2);
                        else
                            Pointer += 3;
                    }
                    else if (op == 7)
                    {
                        SetValue(a, Pointer + 3, GetValue(c, Pointer + 1) < GetValue(b, Pointer + 2) ? 1 : 0);
                        Pointer += 4;
                    }
                    else if (op == 8)
                    {
                        SetValue(a, Pointer + 3, GetValue(c, Pointer + 1) == GetValue(b, Pointer + 2) ? 1 : 0);
                        Pointer += 4;
                    }
                    else if (op == 9)
                    {
                        RelativeBase += (int)GetValue(c, Pointer + 1);
                        Pointer += 2;
                    }
                    else if (op == 99)
                    {
                        Finished = true;
                        running = false;
                    }
                    else
                    {
                        throw new ApplicationException("I fucked up");
                    }
                }

                if (Program[Pointer] % 100 == 99) //Is next inst halt?
                    Finished = true;

                return outputValue;
            }


            public void SetValue(int mode, long address, long value)
            {
                if (mode == 0) //Position
                    SetValue(GetValue(address), value);
                else if (mode == 1) //immediate 
                    SetValue(address, value);
                else if (mode == 2) //Relative
                    SetValue(GetValue(address) + RelativeBase, value);
                else
                    throw new ApplicationException($"{mode} is not a valid mode");
            }

            private void SetValue(long address, long value)
            {
                while (Program.Count <= address)
                    Program.Add(0);

                Program[(int)address] = value;
            }

            public long GetValue(int mode, long pointer)
            {
                var value = GetValue(pointer);

                if (mode == 0) //Position
                    return GetValue(value);
                else if (mode == 1) //immediate 
                    return value;
                else if (mode == 2) //Relative
                    return GetValue(value + RelativeBase);
                else
                    throw new ApplicationException($"{mode} is not a valid mode");
            }

            public long GetValue(long address)
            {
                return Program.Count > address ? Program[(int)address] : 0;
            }
        }
    }
}