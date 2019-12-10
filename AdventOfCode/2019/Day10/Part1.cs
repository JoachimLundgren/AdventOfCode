using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;

namespace AdventOfCode2019.Day10
{
    public class Part1
    {
        public static void Run()
        {
            var temp = GetCoordinatesBetweenCoordinates(new Coordinate(1, 1), new Coordinate(4, 4));


            var input = File.ReadAllLines("2019/Day10/Input.txt");
            var map = input.Select(str => str.ToList().Select(c => c == '#').ToList()).ToList();

            var best = 0;
            for (int y = 0; y < map.Count; y++)
            {
                for (int x = 0; x < map.First().Count; x++)
                {
                    if (map[y][x])
                    {
                        var current = GetNumberOfVisibleAstroids(new Coordinate(x, y), map);

                        if (current > best)
                            best = current;
                    }
                }
            }

            Console.WriteLine(best);
        }

        private static int GetNumberOfVisibleAstroids(Coordinate orgin, List<List<bool>> map)
        {
            var result = 0;
            for (int y1 = 0; y1 < map.Count; y1++)
            {
                for (int x1 = 0; x1 < map.First().Count; x1++)
                {
                    if (map[y1][x1] && (orgin.X != x1 || orgin.Y != y1))
                    {
                        if (!IsBlockedByOther(orgin, new Coordinate(x1, y1), map))
                            result++;
                    }
                }
            }
            return result;
        }


        private static bool IsBlockedByOther(Coordinate origin, Coordinate target, List<List<bool>> map)
        {
            var coordinates = GetCoordinatesBetweenCoordinates(origin, target);
            foreach (var coordinate in coordinates)
            {
                if (map[coordinate.Y][coordinate.X])
                    return true;
            }
            return false;
        }

        private static List<Coordinate> GetCoordinatesBetweenCoordinates(Coordinate origin, Coordinate target)
        {
            var coordinates = new List<Coordinate>();

            var xSteps = target.X - origin.X;
            var ySteps = target.Y - origin.Y;

            var dist = Math.Abs(xSteps) + Math.Abs(ySteps);

            for (int i = 1; i < dist; i++)
            {
                var x = origin.X + i * xSteps / (double)dist;
                var y = origin.Y + i * ySteps / (double)dist;
                if (x % 1 == 0 && y % 1 == 0)
                    coordinates.Add(new Coordinate((int)x, (int)y));
            }

            return coordinates;
        }


        private class Coordinate
        {
            public int X { get; }
            public int Y { get; }
            public Coordinate(int x, int y)
            {
                X = x;
                Y = y;
            }
        }
    }

}