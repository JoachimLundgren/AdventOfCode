using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;

namespace AdventOfCode2019.Day11
{
    public class Part1
    {
        public static void Run()
        {
            var input = File.ReadAllLines("2019/Day11/Input.txt");

            var program = input.First().Split(',').Select(long.Parse).ToList();
            var computer = new Computer { Program = program.ToList() };

            var painted = new Dictionary<Location, int>();
            var location = new Location();
            var orientation = 0; //0 = up, 1= left, 2 = down, 3 = right
            while (!computer.Finished)
            {
                var output = computer.RunCode();
                if (!computer.Finished)
                {
                    if (!painted.ContainsKey(location))
                        painted.Add(location, output.Color);
                    else
                        painted[location] = output.Color;

                    orientation = Turn(orientation, output.Direction);

                    if (orientation == 0)
                        location = new Location { X = location.X + 1, Y = location.Y };
                    else if (orientation == 1)
                        location = new Location { X = location.X, Y = location.Y - 1 };
                    else if (orientation == 2)
                        location = new Location { X = location.X - 1, Y = location.Y };
                    else if (orientation == 3)
                        location = new Location { X = location.X, Y = location.Y + 1 };
                    else
                        throw new ApplicationException("ooops");

                    computer.NextInput = painted.GetValueOrDefault(location, 0);
                }
            }

            Console.WriteLine(painted.Count);
        }

        private static int Turn(int orientation, int direction)
        {
            if (direction == 0)
                orientation++;
            else if (direction == 1)
                orientation--;
            else
                throw new ApplicationException("hmmm");

            //TODO %?
            if (orientation == -1)
                orientation = 3;
            else if (orientation == 4)
                orientation = 0;

            return orientation;
        }

        private class Computer
        {
            //private int inputPointer;

            public int Pointer { get; set; }
            public List<long> Program { get; set; }
            public bool Finished { get; set; }
            //public List<int> Inputs { get; set; }
            public int RelativeBase { get; set; }

            public int NextInput { get; set; }

            public Output RunCode()
            {
                var running = true;
                Output output = null;

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
                        var outputValue = GetValue(c, Pointer + 1);
                        Pointer += 2;
                        if (output != null)
                        {
                            output.Direction = (int)outputValue;
                            running = false;
                        }
                        else
                        {
                            output = new Output() { Color = (int)outputValue };
                        }
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

                return output;
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

        private class Output
        {
            public int Color { get; set; }
            public int Direction { get; set; }
        }

        private class Location
        {
            public int X { get; set; }
            public int Y { get; set; }

            public override bool Equals(object obj)
            {
                var other = obj as Location;
                return X == other.X && Y == other.Y;
            }
            public override int GetHashCode()
            {
                return X.GetHashCode() ^ Y.GetHashCode();
            }
        }
    }
}