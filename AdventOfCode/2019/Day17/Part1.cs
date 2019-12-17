using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Utils;

namespace AdventOfCode2019.Day17
{
    public class Part1
    {
        public void Run()
        {
            var input = File.ReadAllLines("2019/Day17/Input.txt");
            var program = input.First().Split(',').Select(int.Parse).ToList();
            var computer = new Computer { Program = program.ToList() };

            var x = 0;
            var y = 0;
            var scaffolds = new List<Coordinate>();
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
            }

            var sum = 0;
            foreach (var coordinate in scaffolds)
            {
                if (scaffolds.Any(c => c.X == coordinate.X && c.Y == coordinate.Y - 1)
                    && scaffolds.Any(c => c.X == coordinate.X && c.Y == coordinate.Y + 1)
                    && scaffolds.Any(c => c.X == coordinate.X - 1 && c.Y == coordinate.Y)
                    && scaffolds.Any(c => c.X == coordinate.X + 1 && c.Y == coordinate.Y))
                {
                    sum += coordinate.X * coordinate.Y;
                }
            }
            Console.WriteLine(sum);
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