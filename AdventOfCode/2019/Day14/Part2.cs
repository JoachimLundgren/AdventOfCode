using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdventOfCode.Utils;

namespace AdventOfCode2019.Day14
{
    public class Part2
    {
        private Dictionary<string, Reaction> reactions;
        public void Run()
        {
            var input = File.ReadAllLines("2019/Day14/Input.txt");
            reactions = new Dictionary<string, Reaction>();
            foreach (var row in input)
            {
                var reaction = new Reaction(row);
                reactions.Add(reaction.Target.Name, reaction);
            }

            var chemicalsNeeded = new Dictionary<string, long>();
            var extraChemicals = new Dictionary<string, long>();
            extraChemicals.Add("ORE", 1000000000000);
            var fuelProduced = 0;
            var orePerFuelRatio = new Part1().Run();
            var oldPercentage = -1L;
            var canProduceMore = true;
            while (canProduceMore)  //Slow...takes around 10min for real input
            {
                var percentage = 100 - (extraChemicals["ORE"] / 10000000000);
                if (percentage != oldPercentage)
                {
                    oldPercentage = percentage;
                    Console.SetCursorPosition(0, 4);
                    Console.WriteLine($"{percentage:d2}%");
                }
                chemicalsNeeded.Add("FUEL", 1);
                while (chemicalsNeeded.Any() && canProduceMore)
                {
                    var nextChecmicalsNeeded = new Dictionary<string, long>();
                    foreach (var need in chemicalsNeeded)   //Determine next reactions
                    {
                        if (!need.Key.Equals("ORE"))
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
                                extraChemicals.Add(need.Key, extras);

                        }
                    }
                    chemicalsNeeded = nextChecmicalsNeeded;
                    ResolveChemsFromExtras(chemicalsNeeded, extraChemicals);
                    canProduceMore = orePerFuelRatio <= extraChemicals["ORE"];
                }
                fuelProduced++;
            }
            if (CanCreate(reactions["FUEL"].Target, extraChemicals))
                fuelProduced++;

            Console.WriteLine(fuelProduced); //4076489 too low
        }

        private bool CanCreate(Chemical chem, Dictionary<string, long> extras)
        {
            if (extras.GetValueOrDefault(chem.Name, 0) >= chem.Quantity)
            {
                extras[chem.Name] -= chem.Quantity;
                return true;
            }
            else if (chem.Name.Equals("ORE"))
            {
                return false;
            }
            else
            {
                return reactions[chem.Name].Checmicals.TrueForAll(c => CanCreate(c, extras));
            }
        }

        private void ResolveChemsFromExtras(Dictionary<string, long> chemicalsNeeded, Dictionary<string, long> extraChemicals)
        {
            var somethingProduced = true;
            while (somethingProduced)
            {
                somethingProduced = false;
                foreach (var need in chemicalsNeeded.ToDictionary(kvp => kvp.Key, kvp => kvp.Value))   //Check what we have
                {
                    if (extraChemicals.ContainsKey(need.Key))
                    {
                        somethingProduced = true;
                        if (extraChemicals[need.Key] > chemicalsNeeded[need.Key])
                        {
                            extraChemicals[need.Key] -= chemicalsNeeded[need.Key];
                            chemicalsNeeded.Remove(need.Key);
                        }
                        else if (extraChemicals[need.Key] < chemicalsNeeded[need.Key])
                        {
                            chemicalsNeeded[need.Key] -= extraChemicals[need.Key];
                            extraChemicals.Remove(need.Key);
                        }
                        else
                        {
                            chemicalsNeeded.Remove(need.Key);
                            extraChemicals.Remove(need.Key);
                        }
                    }
                }
            }
        }

        private class Chemical
        {
            public int Quantity { get; }
            public string Name { get; }

            public Chemical(string checmical)
            {
                var parts = checmical.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                Quantity = int.Parse(parts[0]);
                Name = parts[1];
            }
        }

        private class Reaction
        {
            public Chemical Target { get; set; }
            public List<Chemical> Checmicals { get; set; }

            public Reaction(string reaction)
            {
                var splitted = reaction.Split(" => ", StringSplitOptions.RemoveEmptyEntries);
                Target = new Chemical(splitted[1]);
                Checmicals = splitted[0].Split(',').Select(s => new Chemical(s)).ToList();
            }
        }

    }
}