using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode.Utils
{
    public static class MatrixExtensions
    {
        public static void PrintMap(this char[][] map)
        {
            foreach (var row in map)
            {
                Console.WriteLine(string.Join("", row));
            }
        }
    }
}
