using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace AdventOfCode.Day12
{
    public class Part1
    {
        public static void Run()
        {
            var input = File.ReadAllLines("Day12/Input.txt");
            var state = ParseInitialState(input[0]);
            var spread = input.Skip(2).Select(line => new SpreadDefinition(line)).ToList();
            var prepends = 0;

            for (long i = 0; i < 50000000000; i++)
            {
                if (state[0])
                {
                    state = state.Prepend(false).ToArray();
                    prepends++;
                }
                if (state[1])
                {
                    state = state.Prepend(false).ToArray();
                    prepends++;
                }
                if (state[2])
                {
                    state = state.Prepend(false).ToArray();
                    prepends++;
                }

                if (state[state.Length - 1])
                    state = state.Append(false).ToArray();

                if (state[state.Length - 2])
                    state = state.Append(false).ToArray();

                if (state[state.Length - 3])
                    state = state.Append(false).ToArray();


                var newState = new bool[state.Length];
                for (int j = 2; j < state.Length - 2; j++)
                {
                    var pattern = state.Skip(j - 2).Take(5).ToArray();
                    newState[j] = spread.SingleOrDefault(s => s.Match(pattern))?.Output == true;
                }

                state = newState;
            }

            var points = 0;
            for (int i = 0; i < state.Length; i++)
            {
                if (state[i])
                    points += i - prepends;
            }

            Console.WriteLine(points);
        }

        private static void PrintOutput(bool[] state)
        {
            var str = new string(state.Select(s => s ? '#' : '.').ToArray());
            Console.WriteLine(str);
        }

        private static bool[] ParseInitialState(string line)
        {
            return line.Skip(15).Select(c => c == '#').ToArray();
        }


        private class SpreadDefinition
        {
            public bool[] Pattern { get; set; }
            public bool Output { get; set; }

            public SpreadDefinition(string input)
            {
                Pattern = input.Take(5).Select(c => c == '#').ToArray();
                Output = input.Last() == '#';
            }

            public bool Match(bool[] state)
            {
                return state.SequenceEqual(Pattern);
            }
        }
    }
}
