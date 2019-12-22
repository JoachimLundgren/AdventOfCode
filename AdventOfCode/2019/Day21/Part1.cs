using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdventOfCode.Utils;

namespace AdventOfCode2019.Day21
{
    public class Part1
    {
        public void Run()
        {
            var input = File.ReadAllLines("2019/Day21/Input.txt");
            var program = input.First().Split(',').Select(int.Parse).ToList();
            var computer = new Computer(program.ToList());


            var temp = new List<string>()
            {
                "NOT A J",
                "NOT C T",
                "AND D T",
                "OR T J",
                "WALK"
            };

            computer.AddInput(GetAscii(temp));

            int output = 0;
            while (!computer.Finished)
            {
                output = computer.RunCode();
                Console.Write((char)output);
            }

            Console.WriteLine(output);
        }


        private int[] GetAscii(List<string> strings)
        {
            var result = new List<int>();
            foreach (var str in strings)
            {
                result.AddRange(GetAscii(str));
                result.Add(10);
            }
            return result.ToArray();
        }

        private int[] GetAscii(string str)
        {
            return str.Select(c => (int)c).ToArray();
        }

        /*
        private List<List<string>> GenerateFirstCommands()
        {
            var result = new List<List<string>>();

            var commands = GetCommands();
            foreach (var c in commands)
            {
                if (c.Last() == 'J')
                {
                    var newList = new List<string>() { c };
                    newList.Add("WALK");
                    result.Add(newList);
                }
            }

            return result;
        }


        private List<List<string>> GenerateNextCommands(List<List<string>> previous)
        {
            var next = new List<List<string>>();
            var commands = GetCommands();
            foreach (var item in previous)
            {
                foreach (var c in commands)
                {
                    var newList = item.ToList();
                    newList.Insert(0, c);
                    next.Add(newList);
                }
            }

            return next;
        }

        private List<List<string>> GetCommands(bool run = false)
        {
            var result = new List<List<string>>();

            var commands = GetCommands();
            foreach (var c in commands)
            {
                var newList = new List<string>() { c };
                newList.Add(run ? "RUN" : "WALK");
                result.Add(newList);
            }

            return result;
        }

        private List<string> GetCommands()
        {
            var baseCommands = new List<string>() { "AND", "NOT", "OR" };
            var readRegisters = new List<char>() { 'T', 'J', 'A', 'B', 'C', 'D' };
            var writeRegisters = new List<char>() { 'T', 'J' };

            var result = new List<string>();

            foreach (var b in baseCommands)
            {
                foreach (var r in readRegisters)
                {
                    foreach (var w in writeRegisters)
                    {
                        result.Add($"{b} {r} {w}");
                    }
                }
            }

            return result;
        }*/


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