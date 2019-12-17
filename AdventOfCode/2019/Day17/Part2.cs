using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdventOfCode.Utils;

namespace AdventOfCode2019.Day17
{
    public class Part2
    {
        public void Run()
        {
            var input = File.ReadAllLines("2019/Day17/Input.txt");
            var program = input.First().Split(',').Select(int.Parse).ToList();
            var computer = new Computer { Program = program.ToList() };

            var x = 0;
            var y = 0;
            var scaffolds = new List<Coordinate>();
            Coordinate robot = null;
            while (!computer.Finished)
            {
                var output = computer.RunCode(0);
                Console.Write((char)output);
                if (output == 35)
                {
                    scaffolds.Add(new Coordinate(x, y));
                    x++;
                }
                else if (output == 46)
                {
                    x++;
                }
                else if (output == 10)
                {
                    x = 0;
                    y++;
                }
                else if (output == 94)  // ^
                {
                    robot = new Coordinate(x, y);
                    x++;
                }
            }

            program[0] = 2;
            Coordinate robotDirection = new Coordinate(0, -1);
            var turn = 'X';
            var count = 0;
            var path = new List<Tuple<char, int>>();
            while (scaffolds.Count > 1)
            {
                var adjacent = GetAdjacentCoordinates(robot, scaffolds);
                var next = adjacent.FirstOrDefault(a => a.X == robot.X + robotDirection.X && a.Y == robot.Y + robotDirection.Y);
                if (next != null)
                {
                    count++;
                    if (adjacent.Count != 3) //No crossing
                        scaffolds.Remove(robot);

                    robot = next;
                }
                else
                {
                    if (count != 0)
                        path.Add(new Tuple<char, int>(turn, count));

                    next = adjacent.Single();
                    var newDirection = new Coordinate(next.X - robot.X, next.Y - robot.Y);
                    turn = GetTurn(robotDirection, newDirection);
                    robotDirection = newDirection;
                    count = 0;
                }

                Console.SetCursorPosition(robot.X, robot.Y + 2);
                Console.Write(turn);
            }
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine(string.Join(" ", path.Select(p => $"{p.Item1}{p.Item2}")));

            var combinations = GenerateCombinations(path.ToList());
            var comparer = new ListComparer<Tuple<char, int>>();
            var temp = combinations.Distinct(comparer).ToList();
            var results = combinations.Select(c => c.Distinct().ToList()).Where(c => c.Count == 3).ToList();

        }

        private List<List<T>> GenerateCombinations<T>(List<T> list, int maxSize = 7)
        {
            var result = new List<List<T>>();
            for (int i = 0; i < maxSize; i++)
            {
                result.Add(list.Take(i).ToList());
                result.AddRange(GenerateCombinations(list.Skip(i).ToList(), maxSize - 1));
            }
            return result;
        }

        private bool IsValid(List<Tuple<char, int>> first, List<Tuple<char, int>> second, List<Tuple<char, int>> third)
        {
            return first.Count <= 10 && second.Count <= 10 && third.Count <= 10;
        }

        private List<Coordinate> GetAdjacentCoordinates(Coordinate coordinate, List<Coordinate> scaffolds)
        {
            return scaffolds.Where(c => c.X == coordinate.X && c.Y == coordinate.Y - 1
                                        || c.X == coordinate.X && c.Y == coordinate.Y + 1
                                        || c.X == coordinate.X - 1 && c.Y == coordinate.Y
                                        || c.X == coordinate.X + 1 && c.Y == coordinate.Y).ToList();
        }

        private char GetTurn(Coordinate robotDirection, Coordinate newDirection)
        {
            if (robotDirection.X == -1 && robotDirection.Y == 0) //going left
            {
                if (newDirection.Y == -1) //turn left
                    return 'R';
                else if (newDirection.Y == 1)  //turn right
                    return 'L';
            }
            else if (robotDirection.X == 1 && robotDirection.Y == 0) //going right
            {
                if (newDirection.Y == -1)
                    return 'L';
                else if (newDirection.Y == 1)
                    return 'R';
            }
            else if (robotDirection.X == 0 && robotDirection.Y == -1)
            {
                if (newDirection.X == -1)
                    return 'L';
                else if (newDirection.X == 1)
                    return 'R';
            }
            else if (robotDirection.X == 0 && robotDirection.Y == 1)
            {
                if (newDirection.X == -1)
                    return 'R';
                else if (newDirection.X == 1)
                    return 'L';
            }

            throw new Exception("hmm");
        }

        private class ListComparer<T> : EqualityComparer<List<T>>
        {
            public override bool Equals(List<T> x, List<T> y)
            {
                return x.SequenceEqual(y);
            }

            public override int GetHashCode(List<T> obj)
            {
                unchecked
                {
                    int hash = 19;
                    foreach (var o in obj)
                    {
                        hash = hash * 31 + o.GetHashCode();
                    }
                    return hash;
                }
            }
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