using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace AdventOfCode2019.Day12
{
    public class Part2
    {
        public static void Run()
        {
            var input = File.ReadAllLines("2019/Day12/Input.txt");

            var moons = new List<Moon>();
            foreach (var arr in input.Select(i => i.Trim('<', '>').Split(", ")).ToList())
            {
                moons.Add(new Moon(int.Parse(arr.ElementAt(0).Substring(2)), int.Parse(arr.ElementAt(1).Substring(2)), int.Parse(arr.ElementAt(2).Substring(2))));
            }
            var x = moons.Select(m => new Coordinate() { Position = m.Position.X }).ToList();
            var y = moons.Select(m => new Coordinate() { Position = m.Position.Y }).ToList();
            var z = moons.Select(m => new Coordinate() { Position = m.Position.Z }).ToList();

            var xresult = GetIterationsForCoordinate(x);
            var yresult = GetIterationsForCoordinate(y);
            var zresult = GetIterationsForCoordinate(z);
            var lcm = LeastCommonMultiplier(xresult, yresult, zresult);
            Console.WriteLine($"{xresult} {yresult} {zresult} => {lcm}");
        }

        private static int GetIterationsForCoordinate(List<Coordinate> coordinates)
        {
            var coordinatesCopy = coordinates.Select(c => new Coordinate() { Position = c.Position, Velocity = c.Velocity }).ToList();
            var allBack = false;
            var iterations = 0;

            while (!allBack)
            {
                for (int i = 0; i < 4; i++)
                {
                    for (int j = i + 1; j < 4; j++)
                    {
                        ApplyGravity(coordinates[i], coordinates[j]);
                    }
                }

                for (int i = 0; i < 4; i++)
                {
                    ApplyVelocity(coordinates[i]);
                }

                allBack = coordinates[0].Position == coordinatesCopy[0].Position && coordinates[0].Velocity == coordinatesCopy[0].Velocity
                        && coordinates[1].Position == coordinatesCopy[1].Position && coordinates[1].Velocity == coordinatesCopy[1].Velocity
                        && coordinates[2].Position == coordinatesCopy[2].Position && coordinates[2].Velocity == coordinatesCopy[2].Velocity
                        && coordinates[3].Position == coordinatesCopy[3].Position && coordinates[3].Velocity == coordinatesCopy[3].Velocity;

                iterations++;
            }

            return iterations;
        }

        private static long LeastCommonMultiplier(params long[] numbers)
        {
            return numbers.Aggregate(LeastCommonMultiplier);
        }

        private static long LeastCommonMultiplier(long a, long b)
        {
            return Math.Abs(a * b) / GreatestCommonDivisor(a, b);
        }

        private static long GreatestCommonDivisor(long a, long b)
        {
            return b == 0 ? a : GreatestCommonDivisor(b, a % b);
        }

        private static void ApplyGravity(Coordinate first, Coordinate second)
        {
            if (first.Position > second.Position)
            {
                first.Velocity--;
                second.Velocity++;
            }
            else if (first.Position < second.Position)
            {
                first.Velocity++;
                second.Velocity--;
            }
        }

        private static void ApplyVelocity(Coordinate c)
        {
            c.Position += c.Velocity;
        }

        private class Coordinate
        {
            public int Position { get; set; }
            public int Velocity { get; set; }
        }
        private class Moon
        {
            public Coordinates Position { get; set; }
            public Coordinates Velocity { get; set; }

            public int TotalEnergy => Position.Energy * Velocity.Energy;

            public Moon(int x, int y, int z)
            {
                Position = new Coordinates() { X = x, Y = y, Z = z };
                Velocity = new Coordinates();
            }
        }

        private class Coordinates
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int Z { get; set; }

            public int Energy => Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z);
        }
    }
}