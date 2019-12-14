using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using AdventOfCode.Utils;

namespace AdventOfCode2019.Day14
{
    public class Part1
    {
        public int Run()
        {
            var input = File.ReadAllLines("2019/Day14/Input.txt");
            var reactions = new Dictionary<string, Reaction>();
            foreach (var row in input)
            {
                var reaction = new Reaction(row);
                reactions.Add(reaction.Target.Name, reaction);
            }

            var checmicalsNeeded = new Dictionary<string, int>();
            checmicalsNeeded.Add("FUEL", 1);
            var extraChecmicals = new Dictionary<string, int>();
            var oreNeeded = 0;
            while (checmicalsNeeded.Any())
            {
                foreach (var need in checmicalsNeeded.ToDictionary(kvp => kvp.Key, kvp => kvp.Value))   //Check what we have
                {
                    if (extraChecmicals.ContainsKey(need.Key))
                    {
                        if (extraChecmicals[need.Key] > checmicalsNeeded[need.Key])
                        {
                            extraChecmicals[need.Key] -= checmicalsNeeded[need.Key];
                            checmicalsNeeded.Remove(need.Key);
                        }
                        else if (extraChecmicals[need.Key] < checmicalsNeeded[need.Key])
                        {
                            checmicalsNeeded[need.Key] -= extraChecmicals[need.Key];
                            extraChecmicals.Remove(need.Key);
                        }
                        else
                        {
                            checmicalsNeeded.Remove(need.Key);
                            extraChecmicals.Remove(need.Key);
                        }
                    }
                }

                var nextChecmicalsNeeded = new Dictionary<string, int>();
                foreach (var need in checmicalsNeeded)   //Determine next reactions
                {
                    if (need.Key.Equals("ORE"))
                    {
                        oreNeeded += need.Value;
                    }
                    else
                    {
                        var reaction = reactions[need.Key];
                        var numberOfTimeToRunReaction = (need.Value + reaction.Target.Quantity - 1) / reaction.Target.Quantity;
                        var extras = numberOfTimeToRunReaction * reaction.Target.Quantity - need.Value;

                        foreach (var chem in reaction.Checmicals)
                        {
                            if (nextChecmicalsNeeded.ContainsKey(chem.Name))
                                nextChecmicalsNeeded[chem.Name] += chem.Quantity * numberOfTimeToRunReaction;
                            else
                                nextChecmicalsNeeded.Add(chem.Name, chem.Quantity * numberOfTimeToRunReaction);
                        }

                        if (extras > 0)
                            extraChecmicals.Add(need.Key, extras);

                    }
                }
                checmicalsNeeded = nextChecmicalsNeeded;
            }

            Console.WriteLine(oreNeeded);
            return oreNeeded;
        }


        private class Checmical
        {
            public int Quantity { get; }
            public string Name { get; }

            public Checmical(string checmical)
            {
                var parts = checmical.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                Quantity = int.Parse(parts[0]);
                Name = parts[1];
            }
        }

        private class Reaction
        {
            public Checmical Target { get; set; }
            public List<Checmical> Checmicals { get; set; }

            public Reaction(string reaction)
            {
                var splitted = reaction.Split(" => ", StringSplitOptions.RemoveEmptyEntries);
                Target = new Checmical(splitted[1]);
                Checmicals = splitted[0].Split(',').Select(s => new Checmical(s)).ToList();
            }
        }

    }
}