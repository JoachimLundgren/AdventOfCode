using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace AdventOfCode.Day2
{
    public class Part2
    {
        public static void Run()
        {
            var input = File.ReadAllLines("Day2/Input.txt");

            for (int i = 0; i < input.Length; i++)
            {
                var line = input[i];

                for (int j = 0; j < i; j++)
                {
                    if(IsAlmostEqual(input[i], input[j], out int result))
                    {
                        StringBuilder sb = new StringBuilder(input[i]);
                        sb.Remove(result, 1);
                        Console.WriteLine(sb);
                        
                    }
                }
            }
        }

        private static bool IsAlmostEqual(string first, string second, out int unequalLetter)
        {
            unequalLetter = -1;
            for (int i = 0; i < first.Length; i++)
            {
                if (first[i] != second[i])
                {
                    if (unequalLetter != -1)
                        return false;
                    unequalLetter = i;
                }
            }
            return true;
        }
    }
}
