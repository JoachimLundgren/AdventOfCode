using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using System.Drawing;

namespace AdventOfCode.Day7
{
    public class Part2
    {
        public static void Run()
        {
            var input = File.ReadAllLines("Day7/Input.txt");

            var steps = new Dictionary<char, Step>();
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

            var result = 0;

            var stepsInProgress = new List<Step>();
            while (steps.Count > 0)
            {
                if (stepsInProgress.Count < 5)
                {
                    var availableSteps = steps.Values.Where(s => s.Prerequsites.Count == 0).OrderBy(s => s.Name).ToList();
                    foreach (var availableStep in availableSteps)
                    {
                        if (!stepsInProgress.Contains(availableStep) && stepsInProgress.Count < 5)
                            stepsInProgress.Add(availableStep);
                    }
                }

                foreach (var workingStep in stepsInProgress.OrderBy(s => s.Name).ToList())
                {
                    workingStep.WorkNeeded--;
                    if (workingStep.WorkNeeded == 0)
                    {

                        steps.Remove(workingStep.Name);
                        stepsInProgress.Remove(workingStep);
                        foreach (var step in steps.Values)
                        {
                            step.RemovePrerequsite(workingStep.Name);
                        }

                    }
                }

                result++;
            }

            Console.WriteLine(result);
        }



        private class Step
        {
            public int WorkNeeded { get; set; }
            public char Name { get; }
            public List<char> Prerequsites { get; }

            public Step(char name)
            {
                Name = name;
                Prerequsites = new List<char>();
                WorkNeeded = Name - 'A' + 61;
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
