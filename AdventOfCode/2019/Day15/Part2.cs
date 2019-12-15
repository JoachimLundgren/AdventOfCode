using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdventOfCode.Utils;

namespace AdventOfCode2019.Day15
{
    public class Part2
    {
        public void Run()
        {
            var input = File.ReadAllLines("2019/Day15/Input.txt");
            var program = input.First().Split(',').Select(int.Parse).ToList();
            var computer = new Computer { Program = program.ToList() };

            var map = CreateMap(computer);

            var oxygen = map.Single(c => c.Value == 2).Key;

            var walls = map.Where(m => m.Value == 0).ToDictionary(m => m.Key, m => 0);
            VisitAllCoordinates(oxygen, walls);

            var distance = walls.Max(w => w.Value);

            Console.WriteLine(distance);
        }

        private void VisitAllCoordinates(Coordinate current, Dictionary<Coordinate, int> map, int count = 0)
        {
            map.Add(current, count);

            var moves = GetPossibleMoves(current, map);
            foreach (var move in moves)
            {
                var next = Move(current, move);
                VisitAllCoordinates(next, map, count + 1);
            }
        }

        private Dictionary<Coordinate, int> CreateMap(Computer computer)
        {
            var coordinates = new Dictionary<Coordinate, int>();
            var moveHistory = new List<int>();
            var currentCoordinate = new Coordinate(50, 25);
            coordinates.Add(currentCoordinate, -1);
            Console.SetCursorPosition(currentCoordinate.X, currentCoordinate.Y);
            Console.WriteLine('D');
            while (true)
            {
                var moves = GetPossibleMoves(currentCoordinate, coordinates);
                if (moves.Count == 0) //dead end, go back!
                {
                    if (moveHistory.Any())
                    {
                        var lastMove = moveHistory.Last();

                        if (lastMove == 1) moves.Add(2);
                        if (lastMove == 2) moves.Add(1);
                        if (lastMove == 3) moves.Add(4);
                        if (lastMove == 4) moves.Add(3);

                        moveHistory.RemoveAt(moveHistory.Count - 1);
                    }
                    else
                    {
                        return coordinates;
                    }
                }

                var move = moves.First();
                var output = computer.RunCode(move);

                if (output == 0) //wall
                {
                    var wall = Move(currentCoordinate, move);

                    coordinates.Add(wall, output);
                    Console.SetCursorPosition(wall.X, wall.Y);
                    Console.WriteLine('#');
                }
                else if (output == 1) //Normal move
                {
                    currentCoordinate = Move(currentCoordinate, move);

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
                    currentCoordinate = Move(currentCoordinate, move);

                    coordinates.Add(currentCoordinate, output);
                    Console.SetCursorPosition(currentCoordinate.X, currentCoordinate.Y);
                    Console.WriteLine('o');
                    moveHistory.Add(move);
                }
            }

            return coordinates;
        }

        private Coordinate Move(Coordinate origin, int move)
        {
            if (move == 1) return new Coordinate(origin.X, origin.Y - 1);
            if (move == 2) return new Coordinate(origin.X, origin.Y + 1);
            if (move == 3) return new Coordinate(origin.X - 1, origin.Y);
            if (move == 4) return new Coordinate(origin.X + 1, origin.Y);

            throw new Exception("oops");
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