using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Drawing;

namespace AdventOfCode2019.Day3
{
    public class Part1
    {
        public static void Run()
        {
            var input = File.ReadAllLines("2019/Day3/Input.txt");

            var line0 = GetCoordinates(input[0]);
            var line1 = GetCoordinates(input[1]);

            var intersects = line0.Intersect(line1).ToList();

            var distances = intersects.Select(p => Math.Abs(p.Y) + Math.Abs(p.X)).ToList();
            distances.Remove(0);
            Console.WriteLine(distances.Min());
        }

        private static List<Point> GetCoordinates(string line)
        {
            var latestCoord = new Point(0, 0);
            var pathCoords = new List<Point>() { latestCoord };

            foreach (var instruction in line.Split(','))
            {
                var inst = instruction.First();
                var length = int.Parse(new String(instruction.Skip(1).ToArray()));

                for (int i = 0; i < length; i++)
                {
                    if (inst == 'U')
                        latestCoord = new Point(latestCoord.X + 1, latestCoord.Y);
                    else if (inst == 'D')
                        latestCoord = new Point(latestCoord.X - 1, latestCoord.Y);
                    else if (inst == 'L')
                        latestCoord = new Point(latestCoord.X, latestCoord.Y - 1);
                    else if (inst == 'R')
                        latestCoord = new Point(latestCoord.X, latestCoord.Y + 1);

                    pathCoords.Add(latestCoord);
                }
            }

            return pathCoords;
        }
    }
}
