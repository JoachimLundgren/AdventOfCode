using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;

namespace AdventOfCode.Day10
{
    public class Part1
    {
        public static void Run()
        {
            var input = File.ReadAllLines("Day10/Input.txt");
            var somethings = input.Select(s => new Something(s)).ToList();

            var previousOutput = int.MaxValue;


            while (true)
            {
                somethings.ForEach(s => s.Advance());
                var output = VerifyOutput(somethings);
                if (previousOutput > output)
                {
                    previousOutput = output;
                }
                else
                {
                    somethings.ForEach(s => s.Reverse());
                    break;
                }
            }


            PrintOutput(somethings);
            Console.WriteLine();
        }


        private static int VerifyOutput(List<Something> somethings)
        {
            return somethings.Max(s => s.Position.X) - somethings.Min(s => s.Position.X);
        }

        private static void PrintOutput(List<Something> somethings)
        {
            var left = somethings.Min(s => s.Position.X);
            var right = somethings.Max(s => s.Position.X);
            var top = somethings.Min(s => s.Position.Y);
            var bottom = somethings.Max(s => s.Position.Y);

            var points = new HashSet<Point>(somethings.Select(s => s.Position));

            for (int i = top; i <= bottom; i++)
            {
                var row = string.Empty;
                for (int j = left; j <= right; j++)
                {
                    row += points.Contains(new Point(j, i)) ? '#' : '.';
                }
                Console.WriteLine(row);
            }


        }

        private class Something
        {
            private Point position;
            private Point velocity;
            public Point Position => position;

            public Something(string input)
            {
                var x = int.Parse(input.Substring(10, 6).Trim(' '));
                var y = int.Parse(input.Substring(18, 6).Trim(' '));
                var dx = int.Parse(input.Substring(36, 2).Trim(' '));
                var dy = int.Parse(input.Substring(40, 2).Trim(' '));
                position = new Point(x, y);
                velocity = new Point(dx, dy);
            }

            public void Advance()
            {
                position.X += velocity.X;
                position.Y += velocity.Y;
            }
            public void Reverse()
            {
                position.X -= velocity.X;
                position.Y -= velocity.Y;
            }
        }
    }
}
