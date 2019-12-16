using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Utils;

namespace AdventOfCode2019.Day16
{
    public class Part1
    {
        List<int> basePattern = new List<int>() { 0, 1, 0, -1 };
        public void Run()
        {
            var input = File.ReadAllLines("2019/Day16/Input.txt");
            var signal = input.Single().Select(c => (int)char.GetNumericValue(c)).ToList();
            var nextSignal = new List<int>();
            for (int x = 0; x < 100; x++)
            {
                nextSignal.Clear();
                for (int i = 1; i <= signal.Count; i++)
                {
                    var row = 0;
                    for (int j = 0; j < signal.Count; j++)
                    {
                        var mutiliper = GetMultiplier(i, j + 1);
                        //Console.Write($"{signal[j]}*{mutiliper} + ");
                        var value = mutiliper * signal[j];
                        row += value;
                    }
                    nextSignal.Add(Math.Abs(row) % 10);
                    //Console.WriteLine($" = {Math.Abs(row) % 10}");
                }
                signal = nextSignal.ToList();
            }

            Console.WriteLine(string.Join("", nextSignal.Take(8)));
        }


        private int GetMultiplier(int phase, int offset)
        {
            var index = offset / phase;
            return basePattern[(index) % 4];
        }
    }
}