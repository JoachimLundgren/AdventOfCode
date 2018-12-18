using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using System.Drawing;

namespace AdventOfCode.Day3
{
    public class Part2
    {
        private static Regex numbersRegex = new Regex(@"\d+");
        public static void Run()
        {
            var input = File.ReadAllLines("Day3/Input.txt");
            var elves = input.Select(i => new ElfClaim(i)).ToHashSet();

            var maxWidth = elves.Max(e => e.Left + e.Width);
            var maxHeight = elves.Max(e => e.Top + e.Height);

            var groupedClaims = elves.SelectMany(e => e.Claims).GroupBy(p => p);
            //var groupedClaims = allClaims.GroupBy(p => p);
            var singleClaims = groupedClaims.Where(group => group.Count() == 1).Select(group => group.Key).ToHashSet();

            foreach (var elf in elves)
            {
                if (!elf.Claims.Except(singleClaims).Any())
                {
                    Console.WriteLine(elf.Id);
                    break;
                }
            }
        }



        private class ElfClaim
        {
            public int Id { get; }
            public int Left { get; }
            public int Top { get; }
            public int Width { get; }
            public int Height { get; }
            public HashSet<Point> Claims { get; }
            public ElfClaim(string input)
            {
                var matches = numbersRegex.Matches(input);
                Id = int.Parse(matches[0].Value);
                Left = int.Parse(matches[1].Value);
                Top = int.Parse(matches[2].Value);
                Width = int.Parse(matches[3].Value);
                Height = int.Parse(matches[4].Value);
                Claims = GetClaims();
            }

            private HashSet<Point> GetClaims()
            {
                var points = new HashSet<Point>();

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
