using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;

namespace AdventOfCode2018.Day3
{
    public class Part1
    {
        private static Regex numbersRegex = new Regex(@"\d+");
        public static void Run()
        {
            var input = File.ReadAllLines("2018/Day3/Input.txt");
            var elves = input.Select(i => new ElfClaim(i)).ToList();

            var maxWidth = elves.Max(e => e.Left + e.Width);
            var maxHeight = elves.Max(e => e.Top + e.Height);

            var allClaims = elves.SelectMany(e => e.GetClaims()).ToList();
            var groupedClaims = allClaims.GroupBy(p => p);
            var duplicateClaims = groupedClaims.Where(group => group.Count() > 1);
            Console.WriteLine(duplicateClaims.Count());
        }



        private class ElfClaim
        {
            public int Left { get; set; }
            public int Top { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }

            public ElfClaim(string input)
            {
                var matches = numbersRegex.Matches(input);
                Left = int.Parse(matches[1].Value);
                Top = int.Parse(matches[2].Value);
                Width = int.Parse(matches[3].Value);
                Height = int.Parse(matches[4].Value);
            }

            public IEnumerable<Point> GetClaims()
            {
                var points = new List<Point>();

                for (int i = Left; i < Left + Width; i++)
                {
                    for (int j = Top; j < Top + Height; j++)
                    {
                        points.Add(new Point(i, j));
                    }
                }

                return points;
            }
        }
    }
}
