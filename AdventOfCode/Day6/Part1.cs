using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;

namespace AdventOfCode.Day6
{
    public class Part1
    {
        public static void Run()
        {
            var input = File.ReadAllLines("Day6/Input.txt");
            var fixedCoordinates = input.Select(i => GetCoordinate(i)).ToList();

            var top = fixedCoordinates.Min(c => c.Y);
            var bottom = fixedCoordinates.Max(c => c.Y);
            var left = fixedCoordinates.Min(c => c.X);
            var right = fixedCoordinates.Max(c => c.X);

            var areas = fixedCoordinates.ToDictionary(c => c, c => 0);
            var infinateAreas = new List<Point>();

            for (int i = top; i <= bottom; i++)
            {
                for (int j = left; j <= right; j++)
                {
                    var coordinatesWithDist = fixedCoordinates.Select(c => ManhattanDistance(c, j, i)).OrderBy(t => t.Item2).ToList();
                    if (coordinatesWithDist[0].Item2 < coordinatesWithDist[1].Item2)
                    {
                        if (i == top || i == bottom || j == left || j == right)   //Living on the edge
                        {
                            if (!infinateAreas.Contains(coordinatesWithDist[0].Item1))
                                infinateAreas.Add(coordinatesWithDist[0].Item1);
                        }
                        else
                        {
                            areas[coordinatesWithDist[0].Item1]++;
                        }
                    }
                }
            }
            foreach (var infArea in infinateAreas)
            {
                areas.Remove(infArea);
            }

            Console.WriteLine(areas.Values.Max());  //3261 is too high
        }

        private static Tuple<Point, int> ManhattanDistance(Point first, int x, int y)
        {
            var dist = Math.Abs(first.X - x) + Math.Abs(first.Y - y);
            return Tuple.Create(first, dist);
        }

        private static Point GetCoordinate(string input)
        {
            var parts = input.Split(',');
            return new Point(int.Parse(parts[0]), int.Parse(parts[1]));
        }
    }
}
