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

            var position = 5755; //2020L;
            var size = 10007; // 119315717514047;
            var shuffleTimes = 2; // 101741582076661;

            var newPos = Shuffle(input, position, size);
            var newPos2 = Shuffle(input, newPos, size);
            var deltapos = newPos - position;

            
            //var biggerPosition = bigPosition + deltapos * shuffleTimes;
            var test = new BigInteger(position) + new BigInteger(deltapos) * new BigInteger(shuffleTimes) % size;

            //bigPosition = (bigPosition + deltapos * shuffleTimes) % size;


            Console.WriteLine(test); //85171953555746, 93867250878806 too high
                                     //86997529434425
        }

        private long Shuffle(string[] input, long position, long size)
        {
            foreach (var line in input.Reverse())
            {
                if (line.Equals("deal into new stack"))
                {
                    position = size - position - 1;
                }
                else if (line.StartsWith("deal with increment"))
                {
                    var increment = long.Parse(line.Substring(20));

                    var n = 1;
                    while ((n * size + position) % increment != 0)
                        n++;

                    position = (n * size + position) / increment;
                }
                else if (line.StartsWith("cut"))
                {
                    var cut = long.Parse(line.Substring(4));
                    if (cut < 0)
                        cut = size + cut;

                    position = (position + cut) % size;
                }
            }
            return position;
        }
    }
}