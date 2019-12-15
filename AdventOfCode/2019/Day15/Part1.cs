using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Utils;

namespace AdventOfCode2019.Day15
{
    public class Part1
    {
        public void Run()
        {
            var input = File.ReadAllLines("2019/Day15/Input.txt");
            var program = input.First().Split(',').Select(int.Parse).ToList();
            var computer = new Computer { Program = program.ToList() };

            var moveHistory = new List<int>();

            var oxygenSystemFound = false;
            var random = new Random();

            var coordinates = new Dictionary<Coordinate, int>();
            var currentCoordinate = new Coordinate(50, 50);
            coordinates.Add(currentCoordinate, -1);

            Console.SetCursorPosition(currentCoordinate.X, currentCoordinate.Y);
            Console.WriteLine('D');

            while (!computer.Finished && !oxygenSystemFound)
            {
                var moves = GetPossibleMoves(currentCoordinate, coordinates);
                if (moves.Count == 0) //dead end, go back!
                {
                    var lastMove = moveHistory.Last();

                    if (lastMove == 1) moves.Add(2);
                    if (lastMove == 2) moves.Add(1);
                    if (lastMove == 3) moves.Add(4);
                    if (lastMove == 4) moves.Add(3);

                    moveHistory.RemoveAt(moveHistory.Count - 1);
                }

                var move = moves.First();
                var output = computer.RunCode(move);

                if (output == 0) //wall
                {
                    Coordinate wall = null;
                    if (move == 1) wall = new Coordinate(currentCoordinate.X, currentCoordinate.Y - 1);
                    if (move == 2) wall = new Coordinate(currentCoordinate.X, currentCoordinate.Y + 1);
                    if (move == 3) wall = new Coordinate(currentCoordinate.X - 1, currentCoordinate.Y);
                    if (move == 4) wall = new Coordinate(currentCoordinate.X + 1, currentCoordinate.Y);

                    coordinates.Add(wall, output);
                    Console.SetCursorPosition(wall.X, wall.Y);
                    Console.WriteLine('#');
                }
                else if (output == 1) //Normal move
                {
                    if (move == 1) currentCoordinate = new Coordinate(currentCoordinate.X, currentCoordinate.Y - 1);
                    if (move == 2) currentCoordinate = new Coordinate(currentCoordinate.X, currentCoordinate.Y + 1);
                    if (move == 3) currentCoordinate = new Coordinate(currentCoordinate.X - 1, currentCoordinate.Y);
                    if (move == 4) currentCoordinate = new Coordinate(currentCoordinate.X + 1, currentCoordinate.Y);

                    if (!coordinates.ContainsKey(currentCoordinate)) //Been here!
                    {
                        coordinates.Add(currentCoordinate, output);
                        Console.SetCursorPosition(currentCoordinate.X, currentCoordinate.Y);
                        Console.WriteLine('.');
                        moveHistory.Add(move);
                    }
                }
                else if (output == 2) //Oxygen found!
                {
                    oxygenSystemFound = true;
                    if (move == 1) currentCoordinate = new Coordinate(currentCoordinate.X, currentCoordinate.Y - 1);
                    if (move == 2) currentCoordinate = new Coordinate(currentCoordinate.X, currentCoordinate.Y + 1);
                    if (move == 3) currentCoordinate = new Coordinate(currentCoordinate.X - 1, currentCoordinate.Y);
                    if (move == 4) currentCoordinate = new Coordinate(currentCoordinate.X + 1, currentCoordinate.Y);

                    coordinates.Add(currentCoordinate, output);
                    Console.SetCursorPosition(currentCoordinate.X, currentCoordinate.Y);
                    Console.WriteLine('o');
                    moveHistory.Add(move);
                }
            }

            Console.WriteLine(moveHistory.Count);
        }

        private List<int> GetPossibleMoves(Coordinate current, Dictionary<Coordinate, int> knownCoordinates)
        {
            var result = new List<int>();

            if (!knownCoordinates.Any(kvp => kvp.Key.X == current.X && kvp.Key.Y == current.Y - 1))
                result.Add(1);
            if (!knownCoordinates.Any(kvp => kvp.Key.X == current.X && kvp.Key.Y == current.Y + 1))
                result.Add(2);
            if (!knownCoordinates.Any(kvp => kvp.Key.X == current.X - 1 && kvp.Key.Y == current.Y))
                result.Add(3);
            if (!knownCoordinates.Any(kvp => kvp.Key.X == current.X + 1 && kvp.Key.Y == current.Y))
                result.Add(4);

            return result;
        }


        private class Computer
        {
            public int Pointer { get; set; }
            public List<int> Program { get; set; }
            public bool Finished { get; set; }
            public int RelativeBase { get; set; }

            public int RunCode(int input)
            {
                var running = true;
                var outputValue = 0;

                while (running)
                {
                    var op = Program[Pointer] % 100;
                    var a = Program[Pointer] / 10000 % 10;
                    var b = Program[Pointer] / 1000 % 10;
                    var c = Program[Pointer] / 100 % 10;
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
                        SetValue(c, Pointer + 1, input);
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
                            Pointer = GetValue(b, Pointer + 2);
                        else
                            Pointer += 3;
                    }
                    else if (op == 6)
                    {
                        if (GetValue(c, Pointer + 1) == 0)
                            Pointer = GetValue(b, Pointer + 2);
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
                        RelativeBase += GetValue(c, Pointer + 1);
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


            public void SetValue(int mode, int address, int value)
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

            private void SetValue(int address, int value)
            {
                while (Program.Count <= address)
                    Program.Add(0);

                Program[address] = value;
            }

            public int GetValue(int mode, int pointer)
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

            public int GetValue(int address)
            {
                return Program.Count > address ? Program[address] : 0;
            }
        }
    }
}