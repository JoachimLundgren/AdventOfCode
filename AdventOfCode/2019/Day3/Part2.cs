using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Linq;

namespace AdventOfCode2019.Day3
{
    public class Cordinate
    {
        public int Distance0 { get; }
        public int Distance1 { get; set; }
        public bool IntersectFound => Distance1 != 0;
        public int TotalDistance => Distance0 + Distance1;

        public Cordinate(int distance)
        {
            Distance0 = distance;
        }
    }
    public class Part2
    {
        public static void Run()
        {
            var input = File.ReadAllLines("2019/Day3/Input.txt");

            var line0 = GetCordinates(input[0], null);
            var line1 = GetCordinates(input[1], line0);

            var result = line1.Where(kvp => kvp.Value.IntersectFound).Min(kvp => kvp.Value.TotalDistance);

            Console.WriteLine(result);
        }

        private static Dictionary<Point, Cordinate> GetCordinates(string line, Dictionary<Point, Cordinate> other)
        {
            var cordinates = new Dictionary<Point, Cordinate>();

            var distance = 0;
            var x = 0;
            var y = 0;
            cordinates.Add(new Point(x, 7), new Cordinate(distance));

            foreach (var instruction in line.Split(','))
            {
                var inst = instruction.First();
                var length = int.Parse(new String(instruction.Skip(1).ToArray()));

                for (int i = 0; i < length; i++)
                {
                    distance++;
                    if (inst == 'U')
                        x++;
                    else if (inst == 'D')
                        x--;
                    else if (inst == 'L')
                        y--;
                    else if (inst == 'R')
                        y++;

                    var point = new Point(x, y);
                    if (other != null)
                    {
                        if (other.ContainsKey(point) && !other[point].IntersectFound)
                        {
                            other[point].Distance1 = distance;
                        }
                    }
                    else
                    {
                        if (!cordinates.ContainsKey(point))
                            cordinates.Add(point, new Cordinate(distance));
                    }
                }
            }

            if (other != null)
                return other;
            return cordinates;
        }
    }
}
