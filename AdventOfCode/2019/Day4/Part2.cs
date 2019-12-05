using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Linq;

namespace AdventOfCode2019.Day4
{
    public class Part2
    {
        public static void Run()
        {
            var input = File.ReadAllLines("2019/Day4/Input.txt");
            var range = input.First().Split('-');
            var start = int.Parse(range.First());
            var last = int.Parse(range.Last());
            var potentialNumbers = 0;
            for (int i = start; i < last; i++)
            {
                var numbers = GetIntArray(i);

                if ((numbers[0] <= numbers[1] && numbers[1] <= numbers[2] && numbers[2] <= numbers[3] && numbers[3] <= numbers[4] && numbers[4] <= numbers[5])
                    &&
                    (numbers[0] == numbers[1] && numbers[0] != numbers[2] 
                     || numbers[1] == numbers[2] && numbers[1] != numbers[3] && numbers[1] != numbers[0]
                     || numbers[2] == numbers[3] && numbers[2] != numbers[4] && numbers[2] != numbers[1]
                     || numbers[3] == numbers[4] && numbers[3] != numbers[5] && numbers[3] != numbers[2]
                     || numbers[4] == numbers[5] && numbers[4] != numbers[3]))
                {
                    potentialNumbers++;
                }
            }

            Console.WriteLine(potentialNumbers);    //992 to low, 144686 to high, 1473 wrong 1260 wrong
        }

        private static int[] GetIntArray(int num)
        {
            List<int> listOfInts = new List<int>();
            while (num > 0)
            {
                listOfInts.Add(num % 10);
                num = num / 10;
            }
            listOfInts.Reverse();
            return listOfInts.ToArray();
        }
    }
}
