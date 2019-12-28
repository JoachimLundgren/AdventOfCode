using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdventOfCode.Utils;

namespace AdventOfCode2019.Day25
{
    public class Part1
    {
        private Random rnd = new Random();
        private static bool autoWalk = true;
        private static bool autoPickup = true;

        private List<Room> Rooms = new List<Room>();
        private List<string> items = new List<string>();
        private Room currentRoom;

        private List<string> avoidItems = new List<string>() { "molten lava", "escape pod", "giant electromagnet", "infinite loop", "photons" };

        private static string HelpString =>
            @"north, south, east, or west.
            take <name of item>
            drop <name of item>
            inv";

        public void Run()
        {
            var input = File.ReadAllLines("2019/Day25/Input.txt");
            var program = input.First().Split(',').Select(long.Parse).ToList();
            var computer = new Computer(program.ToList());

            var commands = new Queue<string>();

            while (!computer.Finished)
            {
                string command;
                commands.TryDequeue(out command);

                //Console.WriteLine($"{command} ({currentLocation})");

                var output = RunCommand(computer, command);

                Console.WriteLine(output);

                if (output.Contains("Analysis complete! You may proceed."))
                    return;

                ParseString(output);

                if (currentRoom.Name == "== Security Checkpoint ==" && items.Count == 8)
                {
                    BruteForceThroughDoor(computer);
                }
                else if (!commands.Any())
                {
                    var c = GetNextAutoCommand(output);
                    if (!string.IsNullOrEmpty(c))
                        commands.Enqueue(c);
                    else
                        commands.Enqueue(GetNextManualCommand());
                }
            }

            Console.WriteLine("Game Over!");
        }

        private void BruteForceThroughDoor(Computer computer)
        {
            autoWalk = false;
            autoPickup = false;
            var combos = items.GetAllCombos();

            foreach (var combo in combos)
            {
                foreach (var item in items.ToList())
                {
                    if (!combo.Contains(item))
                        RunCommand(computer, $"drop {item}");
                }

                foreach (var item in combo)
                {
                    if (!items.Contains(item))
                        RunCommand(computer, $"take {item}");
                }

                var res = RunCommand(computer, "east");
                Console.WriteLine(res);
                if (res.Contains("Analysis complete! You may proceed."))
                    return;
            }
        }

        private string RunCommand(Computer computer, string command)
        {
            if (command != null)
            {
                Console.WriteLine(command);
                computer.AddInput(GetAscii(command));
            }

            var str = string.Empty;
            while (true)
            {
                str += (char)computer.RunCode();

                if (str.EndsWith("Command?"))
                {
                    if (command != null) //TODO: Verify if string is valid
                    {
                        if (!str.Contains("You can't go that way") &&
                            !str.Contains("Alert! Droids on this ship are"))
                        {
                            if (command.StartsWith("drop"))
                            {
                                var item = command.Substring(5);
                                if (items.Remove(item))
                                    currentRoom.Items.Add(item);
                            }
                            else if (command.StartsWith("take"))
                            {
                                var item = command.Substring(5);
                                if (currentRoom.Items.Remove(item))
                                    items.Add(item);
                            }
                        }
                    }
                    return str;
                }
                else if (str.EndsWith("airlock."))
                {
                    return str;
                }
            }
        }

        private string GetNextAutoCommand(string str)
        {
            if (autoPickup)
            {
                var pickups = PickUpCommands(str);
                if (pickups.Any())
                {
                    return pickups.First();
                }
                else
                {
                    if (autoWalk)
                    {
                        return GetNextStep();
                    }
                }
            }

            return string.Empty;
        }

        private void ParseString(string str)
        {
            var lines = str.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            if (lines[0].StartsWith("=="))
            {
                if (!Rooms.Any(r => r.Name == lines[0]))
                {
                    Rooms.Add(
                        new Room(lines[0], lines[1], FindOptions(str, "Items here:"), FindOptions(str, "Doors here lead:")));

                }

                if (str.Contains("Alert! Droids on this ship are"))
                    currentRoom = Rooms.Single(r => r.Name == "== Security Checkpoint ==");
                else
                    currentRoom = Rooms.Single(r => r.Name == lines[0]);
            }
            //PrintRooms();
        }

        private string GetNextStep()
        {
            var posibilities = currentRoom.Doors.ToList();

            var index = rnd.Next(posibilities.Count);
            return posibilities[index];
        }

        private List<string> PickUpCommands(string str)
        {
            var items = FindOptions(str, "Items here:");
            return items.Except(avoidItems).Select(item => $"take {item}").ToList();
        }

        private List<string> FindOptions(string str, string headline)
        {
            var options = new List<string>();
            if (str.Contains(headline))
            {
                var substring = str.Substring(str.IndexOf(headline) + headline.Length);
                foreach (var s in substring.Split('\n', StringSplitOptions.RemoveEmptyEntries))
                {
                    if (s.First() == '-')
                    {
                        options.Add(s.Substring(2));
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return options;
        }

        private string GetNextManualCommand()
        {
            while (true)
            {
                var nextInput = Console.ReadLine();
                if (nextInput == "help")
                {
                    Console.WriteLine(HelpString);
                }
                else if (nextInput == "a")
                {
                    autoPickup = !autoPickup;
                    Console.WriteLine($"Autopickup {autoPickup}");
                }
                else
                {
                    return nextInput;
                }
            }
        }

        private int[] GetAscii(string str)
        {
            str += '\n';
            return str.Select(c => (int)c).ToArray();
        }

        private class Room
        {
            public string Name { get; }
            public string Description { get; }

            public List<string> Doors { get; }
            public List<string> Items { get; }
            public List<string> StartItems { get; }

            public Room(string name, string description, List<string> items, List<string> doors)
            {
                Name = name;
                Description = description;
                StartItems = items.ToList();
                Items = items.ToList();
                Doors = doors;
            }
        }


        private class Computer
        {
            private long pointer;
            private int inputPointer;
            private List<int> inputs;
            private List<long> originalProgram;

            private long RelativeBase { get; set; }
            public List<long> Program { get; set; }
            public bool Finished { get; private set; }

            public int NextInput => inputs[inputPointer++];

            public Computer(List<long> program)
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
                this.inputs.AddRange(inputs);
            }

            public long RunCode()
            {
                var running = true;
                var outputValue = 0L;

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

                if (Program[(int)pointer] % 100 == 99) //Is next inst halt?
                    Finished = true;

                return outputValue;
            }


            public void SetValue(long mode, long address, long value)
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

            public long GetValue(long mode, long pointer)
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