using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace AdventOfCode2018.Day12
{
    public class Part2
    {
        public static void Run()
        {
            var input = File.ReadAllLines("2018/Day12/Input.txt");
            var state = ParseInitialState(input[0]);
            var spread = input.Skip(2).Select(line => new SpreadDefinition(line)).ToList();
            var prepends = 0;
            var oldPoints = 0;
            var oldEstimatedPoints = 0l;
            var iterations = 50000000000;
            var sameEstimates = 0;


            for (long i = 1; i <= iterations; i++)
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

                var points = 0;
                for (int a = 0; a < state.Length; a++)
                {
                    if (state[a])
                        points += a - prepends;
                }

                var estimatedPoints = points + (points - oldPoints) * (iterations - i);
                if (oldEstimatedPoints == estimatedPoints)
                {
                    if (sameEstimates == 5)
                    {
                        Console.WriteLine(estimatedPoints);
                        break;
                    }
                    else
                    {
                        sameEstimates++;
                    }
                }
                else
                {
                    sameEstimates = 0;
                }

                oldPoints = points;
                oldEstimatedPoints = estimatedPoints;
            }
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
