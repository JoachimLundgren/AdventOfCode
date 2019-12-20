using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Utils;

namespace AdventOfCode2019.Day19
{
    public class Part2
    {
        public void Run()
        {
            var input = File.ReadAllLines("2019/Day19/Input.txt");
            var program = input.First().Split(',').Select(int.Parse).ToList();

            var x = 4;
            var y = 6;
            var sizeFound = false;
            var computer = new Computer(program.ToList());
            while (!sizeFound)
            {
                var res = Run(computer, x, y);
                //Console.SetCursorPosition(x, y);
                if (res == 1) //Found
                {
                    //Console.Write('#');
                    if (BeamWideEnough(new Coordinate(x, y), computer))
                    {
                        if (BeamHighEnough(new Coordinate(x, y), computer))
                            sizeFound = true;
                        else
                            x++;
                    }
                    else
                    {
                        y++;
                    }
                }
                else
                {
                    //Console.Write('.');
                    x++;
                }

            }

            Console.WriteLine("FOUND!!! " + (x * 10000 + y));   //102350956 wrong
        }

        private bool BeamWideEnough(Coordinate corner, Computer computer)
        {
            for (int i = 0; i < 100; i++)
            {
                if (Run(computer, corner.X + i, corner.Y) != 1)
                    return false;
            }

            return true;
        }
        private bool BeamHighEnough(Coordinate corner, Computer computer)
        {
            for (int i = 0; i < 100; i++)
            {
                if (Run(computer, corner.X, corner.Y + i) != 1)
                    return false;
            }

            return true;
        }

        private int Run(Computer computer, int x, int y)
        {
            int output = 0;
            computer.Reset();
            computer.AddInput(x, y);
            while (!computer.Finished)
            {
                output = computer.RunCode();
            }

            return output;
        }


        private class Computer
        {
            private int pointer;
            private int inputPointer;
            private List<int> inputs;
            private List<int> originalProgram;

            private int RelativeBase { get; set; }
            public List<int> Program { get; set; }
            public bool Finished { get; private set; }

            public int NextInput => inputs[inputPointer++];

            public Computer(List<int> program)
            {
                Program = program;
                originalProgram = program;
                inputs = new List<int>();
            }

            public void Reset()
            {
                Program = originalProgram.ToList();
                pointer = 0;
                inputPointer = 0;
                RelativeBase = 0;
                Finished = false;
                inputs.Clear();
            }

            public void AddInput(params int[] inputs)
            {
                inputPointer = 0;
                this.inputs.AddRange(inputs);
            }

            public int RunCode()
            {
                var running = true;
                var outputValue = 0;

                while (running)
                {
                    var op = Program[pointer] % 100;
                    var a = Program[pointer] / 10000 % 10;
                    var b = Program[pointer] / 1000 % 10;
                    var c = Program[pointer] / 100 % 10;
                    if (op == 1)
                    {
                        SetValue(a, pointer + 3, GetValue(c, pointer + 1) + GetValue(b, pointer + 2));
                        pointer += 4;
                    }
                    else if (op == 2)
                    {

                        SetValue(a, pointer + 3, GetValue(c, pointer + 1) * GetValue(b, pointer + 2));
                        pointer += 4;
                    }
                    else if (op == 3)
                    {
                        SetValue(c, pointer + 1, NextInput);
                        pointer += 2;
                    }
                    else if (op == 4)
                    {
                        outputValue = GetValue(c, pointer + 1);
                        pointer += 2;
                        running = false;
                    }
                    else if (op == 5)
                    {
                        if (GetValue(c, pointer + 1) != 0)
                            pointer = GetValue(b, pointer + 2);
                        else
                            pointer += 3;
                    }
                    else if (op == 6)
                    {
                        if (GetValue(c, pointer + 1) == 0)
                            pointer = GetValue(b, pointer + 2);
                        else
                            pointer += 3;
                    }
                    else if (op == 7)
                    {
                        SetValue(a, pointer + 3, GetValue(c, pointer + 1) < GetValue(b, pointer + 2) ? 1 : 0);
                        pointer += 4;
                    }
                    else if (op == 8)
                    {
                        SetValue(a, pointer + 3, GetValue(c, pointer + 1) == GetValue(b, pointer + 2) ? 1 : 0);
                        pointer += 4;
                    }
                    else if (op == 9)
                    {
                        RelativeBase += GetValue(c, pointer + 1);
                        pointer += 2;
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

                if (Program[pointer] % 100 == 99) //Is next inst halt?
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