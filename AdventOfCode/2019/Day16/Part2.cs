using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdventOfCode.Utils;

namespace AdventOfCode2019.Day16
{
    public class Part2
    {
        List<int> basePattern = new List<int>() { 0, 1, 0, -1 };
        public void Run()
        {
            var input = File.ReadAllLines("2019/Day16/Input.txt");
            var signal = input.Single().Select(c => (int)char.GetNumericValue(c)).ToList();

            var messageOffset = signal.Take(7).Select((t, i) => t * Convert.ToInt32(Math.Pow(10, 7 - i - 1))).Sum();

            signal = Enumerable.Repeat(signal, 10000).SelectMany(i => i).ToList();
            signal = signal.Skip(messageOffset).ToList();

            var nextSignal = new List<int>();

            for (int x = 0; x < 100; x++)
            {
                Console.WriteLine(x);
                nextSignal.Clear();
                for (int i = 1; i <= signal.Count; i++)
                {
                    var row = 0;
                    for (int j = i - 1; j < signal.Count; j++)
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
                //Console.WriteLine(string.Join("", nextSignal));
            }

            Console.WriteLine(string.Join("", nextSignal.Take(8)));
            Console.WriteLine(string.Join("", nextSignal));
            //Console.WriteLine(string.Join("", nextSignal.Skip(messageOffset % nextSignal.Count)));
            //Console.WriteLine(string.Join("", nextSignal.Skip(messageOffset).Take(8)));
        }


        private int GetMultiplier(int phase, int offset)
        {
            var index = offset / phase;
            return basePattern[(index) % 4];
        }
    }
}