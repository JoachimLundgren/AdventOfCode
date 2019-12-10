using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace AdventOfCode2019.Day10
{
    public class Part2
    {
        public static void Run()
        {
            var input = File.ReadAllLines("2019/Day10/Input.txt");

            var g = GetDegree(new Coordinate(11, 13), new Coordinate(11, 12));
            var h = GetDegree(new Coordinate(11, 13), new Coordinate(11, 14));

            var noll = GetDegree(new Coordinate(0, 0), new Coordinate(0, 1));
            var hundraattie = GetDegree(new Coordinate(0, 0), new Coordinate(0, -1));
            var nittio = GetDegree(new Coordinate(0, 0), new Coordinate(1, 0));
            var fortyfem = GetDegree(new Coordinate(0, 0), new Coordinate(1, 1));
            var tvasjuttio = GetDegree(new Coordinate(0, 0), new Coordinate(-1, 0));
            var treettfem = GetDegree(new Coordinate(0, 0), new Coordinate(-1, 1));
            var tvatvafem = GetDegree(new Coordinate(0, 0), new Coordinate(-1, -1));


            var map = new List<Coordinate>(); // input.Select(str => str.ToList().Select(c => c == '#').ToList()).ToList();
            for (int i = 0; i < input.Length; i++)
            {
                for (int j = 0; j < input[i].Length; j++)
                {
                    if (input[i][j] == '#')
                        map.Add(new Coordinate(j, i));
                }
            }


            var best = 0;
            Coordinate bestCoordinate = null;
            foreach (var coordinate in map)
            {
                var current = GetVisibleAsteroids(coordinate, map).Count;

                if (current > best)
                {
                    best = current;
                    bestCoordinate = coordinate;
                }
            }

            Console.WriteLine(best);

            var coordinates = GetVisibleAsteroids(bestCoordinate, map);
            int a = 0;
            while (a < 199)
            {
                coordinates = SortCoordinates(bestCoordinate, coordinates);
                while (a < 199 && coordinates.Any())
                {
                    var c = coordinates[0];
                    map.Remove(c);
                    coordinates.RemoveAt(0);
                    Console.WriteLine($"{a} remove: {c.X},{c.Y}");
                    a++;
                }

                if (a < 199)
                    coordinates = GetVisibleAsteroids(bestCoordinate, map);
            }

            var answer = coordinates[0];
            Console.WriteLine(answer.X * 100 + answer.Y);   //1500 to high
        }

        private static List<Coordinate> SortCoordinates(Coordinate origin, List<Coordinate> coordinates)
        {
            return coordinates.OrderBy(c => GetDegree(origin, c)).ToList();
        }

        private static double GetDegree(Coordinate origin, Coordinate target)
        {
            if (target.X >= origin.X)
                return 90 - ((180 / Math.PI) * Math.Atan((double)(origin.Y - target.Y) / (origin.X - target.X)));
            else
                return 270 - ((180 / Math.PI) * Math.Atan((double)(origin.Y - target.Y) / (origin.X - target.X)));
        }

        private static List<Coordinate> GetVisibleAsteroids(Coordinate origin, List<Coordinate> map)
        {
            var visible = new List<Coordinate>();

            foreach (var coordinate in map)
            {
                if (!IsBlockedByOther(origin, coordinate, map) && (origin.X != coordinate.X || origin.Y != coordinate.Y))
                    visible.Add(coordinate);
            }

            return visible;
        }


        private static bool IsBlockedByOther(Coordinate origin, Coordinate target, List<Coordinate> map)
        {
            var coordinates = GetCoordinatesBetweenCoordinates(origin, target);
            foreach (var coordinate in coordinates)
            {
                if (map.Any(c => c.X == coordinate.X && c.Y == coordinate.Y))
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