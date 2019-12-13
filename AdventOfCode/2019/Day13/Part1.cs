using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;

namespace AdventOfCode2019.Day13
{
    public class Part1
    {
        public static void Run()
        {
            var input = File.ReadAllLines("2019/Day13/Input.txt");

            var program = input.First().Split(',').Select(long.Parse).ToList();
            var computer = new Computer { Program = program.ToList(), Inputs = new List<int>() {  } };
            var grid = new Dictionary<Coordinate, int>();
            while (!computer.Finished)
            {
                var x = (int)computer.RunCode();
                var y = (int)computer.RunCode();
                var id = (int)computer.RunCode();
                var coordinate = new Coordinate { X = x, Y = y };

                if (!grid.ContainsKey(coordinate))
                    grid.Add(coordinate, id);
                else
                    grid[coordinate] = id;
            }

            Console.WriteLine(grid.Values.Count(id => id == 2));
        }


        private class Coordinate
        {
            public int X { get; set; }
            public int Y { get; set; }

            public override bool Equals(object obj)
            {
                var other = obj as Coordinate;
                return X == other.X && Y == other.Y;
            }
        }

        private class Computer
        {
            private int inputPointer;

            public int Pointer { get; set; }
            public List<long> Program { get; set; }
            public bool Finished { get; set; }
            public List<int> Inputs { get; set; }
            public int RelativeBase { get; set; }

            public int NextInput => Inputs[inputPointer++];

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