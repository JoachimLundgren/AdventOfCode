using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Utils;
using System.Threading;
using System.Threading.Tasks;

namespace AdventOfCode2019.Day23
{
    public class Part2
    {
        public void Run()
        {
            var input = File.ReadAllLines("2019/Day23/Input.txt");
            var program = input.First().Split(',').Select(long.Parse).ToList();
            var nics = new Dictionary<int, NIC>();
            for (int i = 0; i < 50; i++)
            {
                nics.Add(i, new NIC(i, program.ToList()));
            }

            //ThreadPool.SetMinThreads(1000, 0);
            var finished = false;

            var natX = 0L;
            var natY = 0L;

            var natys = new List<long>();

            Task.Run(() =>
            {
                while (!finished)
                {
                    if (nics.Values.All(n => n.IsIdle))
                    {
                        if (natys.Contains(natY))
                        {
                            Console.WriteLine(natY);    //26779 too high
                            finished = true;
                        }
                        else
                        {
                            natys.Add(natY);
                            nics[0].EnqueueMessage(new List<long>() { natX, natY });
                        }
                    }

                    Thread.Sleep(10);
                }
            });


            while (!finished)
            {

                foreach (var nic in nics.Values)
                {
                    var dest = (int)nic.Run();
                    if (dest != -1)
                    {
                        var x = nic.Run();
                        var y = nic.Run();

                        if (dest == 255)
                        {
                            natX = x;
                            natY = y;
                        }

                        if (dest < 50)
                        {
                            nics[dest].EnqueueMessage(new List<long>() { x, y });
                        }


                    }
                }
            }
        }

        private class NIC
        {
            public int Id { get; }
            private Computer computer;
            private object syncRoot = new object();

            private Queue<List<long>> inputQueue;
            public bool IsIdle { get; private set; }

            public NIC(int id, List<long> program)
            {
                Id = id;
                inputQueue = new Queue<List<long>>();
                computer = new Computer(program);
                computer.AddInput(id);
                IsIdle = false;
            }

            public long Run()
            {
                lock (syncRoot)
                {
                    if (inputQueue.Count != 0)
                    {
                        computer.AddInput(inputQueue.Dequeue().ToArray());
                    }
                }

                var output = computer.RunCode();
                IsIdle = output == -1;

                return output;
            }

            public void EnqueueMessage(List<long> message)
            {
                lock (syncRoot)
                {
                    inputQueue.Enqueue(message);
                }
            }
        }

        private class Computer
        {
            private long pointer;
            private long inputPointer;
            private List<long> inputs;
            private List<long> originalProgram;

            private long RelativeBase { get; set; }
            public List<long> Program { get; set; }
            public bool Finished { get; private set; }

            public long NextInput => inputs.Count > inputPointer ? inputs[(int)inputPointer++] : -1;

            public Computer(List<long> program)
            {
                Program = program;
                originalProgram = program;
                inputs = new List<long>();
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

            public void AddInput(params long[] inputs)
            {
                this.inputs.AddRange(inputs);
            }

            public long RunCode()
            {
                var running = true;
                var outputValue = 0L;

                var noInput = false;

                while (running)
                {
                    var op = Program[(int)pointer] % 100;
                    var a = Program[(int)pointer] / 10000 % 10;
                    var b = Program[(int)pointer] / 1000 % 10;
                    var c = Program[(int)pointer] / 100 % 10;
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
                        var input = NextInput;
                        if (input == -1)
                        {
                            if (noInput)
                                return -1;
                            else
                                noInput = true;
                        }

                        SetValue(c, pointer + 1, input);
                        pointer += 2;
                    }
                    else if (op == 4)
                    {
                        noInput = false;
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
                        RelativeBase += (int)GetValue(c, pointer + 1);
                        pointer += 2;
                    }
                    else if (op == 99)
                    {
                        noInput = false;
                        Finished = true;
                        running = false;
                    }
                    else
                    {
                        throw new ApplicationException("I fucked up");
                    }
                }

                if (Program[(int)pointer] % 100 == 99) //Is next inst halt?
                    Finished = true;

                return outputValue;
            }


            private void SetValue(long mode, long address, long value)
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

            private long GetValue(long mode, long pointer)
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

            private long GetValue(long address)
            {
                return Program.Count > address ? Program[(int)address] : 0;
            }
        }
    }
}