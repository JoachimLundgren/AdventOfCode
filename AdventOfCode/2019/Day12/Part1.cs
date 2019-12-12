using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;

namespace AdventOfCode2019.Day12
{
    public class Part1
    {
        public static void Run()
        {
            var input = File.ReadAllLines("2019/Day12/Input.txt");
            var moons = new List<Moon>();
            foreach (var arr in input.Select(i => i.Trim('<', '>').Split(", ")).ToList())
            {
                moons.Add(new Moon(int.Parse(arr.ElementAt(0).Substring(2)), int.Parse(arr.ElementAt(1).Substring(2)), int.Parse(arr.ElementAt(2).Substring(2))));
            }

            for (int x = 0; x < 1000; x++)
            {
                for (int i = 0; i < 4; i++)
                {
                    for (int j = i + 1; j < 4; j++)
                    {
                        ApplyGravity(moons[i], moons[j]);
                    }
                }

                for (int i = 0; i < 4; i++)
                {
                    ApplyVelocity(moons[i]);
                }

                //Console.WriteLine($"After {x + 1} steps:");
                //PrintMoons(moons);
            }
            Console.WriteLine(moons.Sum(m => m.TotalEnergy));
        }

        private static void PrintMoons(List<Moon> moons)
        {
            foreach (var moon in moons)
            {
                Console.WriteLine($"pos=<x={moon.Position.X}, y=  {moon.Position.Y}, z= {moon.Position.Z}>, vel=<x= {moon.Velocity.X}, y= {moon.Velocity.Y}, z= {moon.Velocity.Z}>");
            }
        }

        private static void ApplyGravity(Moon first, Moon second)
        {
            if (first.Position.X > second.Position.X)
            {
                first.Velocity.X--;
                second.Velocity.X++;
            }
            else if (first.Position.X < second.Position.X)
            {
                first.Velocity.X++;
                second.Velocity.X--;
            }

            if (first.Position.Y > second.Position.Y)
            {
                first.Velocity.Y--;
                second.Velocity.Y++;
            }
            else if (first.Position.Y < second.Position.Y)
            {
                first.Velocity.Y++;
                second.Velocity.Y--;
            }

            if (first.Position.Z > second.Position.Z)
            {
                first.Velocity.Z--;
                second.Velocity.Z++;
            }
            else if (first.Position.Z < second.Position.Z)
            {
                first.Velocity.Z++;
                second.Velocity.Z--;
            }
        }

        private static void ApplyVelocity(Moon moon)
        {
            moon.Position.X += moon.Velocity.X;
            moon.Position.Y += moon.Velocity.Y;
            moon.Position.Z += moon.Velocity.Z;
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