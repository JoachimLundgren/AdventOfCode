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
            var extraChemicals = new Dictionary<string, long>();
            var chemCopy = new Dictionary<string, long>();
            var oreLeftCopy = 0L;

            var oreLeft = 1000000000000;
            var productionRate = 10000;
            var fuels = 0;
            while (productionRate >= 1)
            {
                while (oreLeft > 0)
                {
                    chemCopy = extraChemicals.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                    oreLeftCopy = oreLeft;
                    oreLeft -= Create("FUEL", productionRate, extraChemicals);
                    fuels += productionRate;
                }

                //Oh crap, reset!
                extraChemicals = chemCopy;
                oreLeft = oreLeftCopy;
                fuels -= productionRate;
                productionRate /= 10;
            }

            Console.WriteLine(fuels);
        }

        private long Create(string name, long needed, Dictionary<string, long> extras)
        {
            if (name.Equals("ORE"))
            {
                return needed;
            }

            if (extras.ContainsKey(name))
            {
                if (extras[name] >= needed)
                {
                    extras[name] -= needed;
                    return 0;
                }
                else
                {
                    needed -= extras[name];
                    extras[name] = 0;
                }
            }
            var count = 0L;
            var reaction = reactions[name];
            var numberOfTimeToRunReaction = (needed + reaction.Target.Quantity - 1) / reaction.Target.Quantity;

            foreach (var chem in reaction.Checmicals)
            {
                count += Create(chem.Name, chem.Quantity * numberOfTimeToRunReaction, extras);
            }
            var created = reaction.Target.Quantity * numberOfTimeToRunReaction;
            if (created > needed)
            {
                if (extras.ContainsKey(name))
                    extras[name] += created - needed;
                else
                    extras.Add(name, created - needed);
            }
            return count;
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