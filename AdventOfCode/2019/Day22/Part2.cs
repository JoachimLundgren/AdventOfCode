using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdventOfCode.Utils;
using System.Numerics;

namespace AdventOfCode2019.Day22
{
    public class Part2
    {
        public void Run()
        {
            var input = File.ReadAllLines("2019/Day22/Input.txt");

            BigInteger position = 2020;
            BigInteger size = 119315717514047;
            BigInteger shuffleTimes = 101741582076661;

            BigInteger offsetPerIncrement = 0;
            BigInteger multiplyerPerIncrement = 1;

            foreach (var line in input)
            {
                if (line.Equals("deal into new stack"))
                {
                    multiplyerPerIncrement *= -1;
                    offsetPerIncrement += multiplyerPerIncrement;
                }
                else if (line.StartsWith("deal with increment"))
                {
                    var dealIncrement = long.Parse(line.Substring(20));
                    multiplyerPerIncrement *= BigInteger.ModPow(dealIncrement, size - 2, size);
                }
                else if (line.StartsWith("cut"))
                {
                    var cut = long.Parse(line.Substring(4));
                    offsetPerIncrement += cut * multiplyerPerIncrement;
                }

                multiplyerPerIncrement = (multiplyerPerIncrement % size + size) % size;
                offsetPerIncrement = (offsetPerIncrement % size + size) % size;
            }

            var increment = BigInteger.ModPow(multiplyerPerIncrement, shuffleTimes, size);
            var offset = offsetPerIncrement * (1 - increment) * BigInteger.ModPow(((1 - multiplyerPerIncrement) % size), size - 2, size);
            offset %= size;

            var result = (offset + position * increment) % size;

            Console.WriteLine(result);
        }
    }
}