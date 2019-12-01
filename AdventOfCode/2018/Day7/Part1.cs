using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;

namespace AdventOfCode2018.Day7
{
    public class Part1
    {
        public static void Run()
        {
            var input = File.ReadAllLines("2018/Day7/Input.txt");

            var steps = new Dictionary<char,Step>();
            foreach (var item in input)
            {
                char prereq = item[5];
                char name = item[36];
                if (!steps.ContainsKey(name))
                    steps.Add(name, new Step(name));

                if (!steps.ContainsKey(prereq))
                    steps.Add(prereq, new Step(prereq));

                steps[name].AddPrerequsite(prereq);
            }

            var result = "";
            while (steps.Count > 0)
            {
                var nextStep = steps.Values.Where(s => s.Prerequsites.Count == 0).OrderBy(s => s.Name).First();
                steps.Remove(nextStep.Name);

                foreach (var step in steps.Values)
                {
                    step.RemovePrerequsite(nextStep.Name);
                }

                result += nextStep.Name;
            }

            Console.WriteLine(result);
        }



        private class Step
        {
            public char Name { get; }
            public List<char> Prerequsites { get; }

            public Step(char name)
            {
                Name = name;
                Prerequsites = new List<char>();
            }

            public void AddPrerequsite(char c)
            {
                if (!Prerequsites.Contains(c))
                    Prerequsites.Add(c);
            }

            public void RemovePrerequsite(char c)
            {
                Prerequsites.Remove(c);
            }
        }
    }
}
