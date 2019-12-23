using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdventOfCode.Utils;

namespace AdventOfCode2019.Day22
{
    public class Part1
    {
        public void Run()
        {
            var input = File.ReadAllLines("2019/Day22/Input.txt");

            var deck = Enumerable.Range(0, 10007).ToList();

            var sum = deck.Sum();

            foreach (var line in input)
            {
                if (line.Equals("deal into new stack"))
                {
                    deck.Reverse();
                }
                else if (line.StartsWith("deal with increment"))
                {
                    var increment = int.Parse(line.Substring(20));
                    var newDeck = new int[deck.Count];
                    for (int i = 0; i < deck.Count; i++)
                    {
                        newDeck[i * increment % deck.Count] = deck[i];
                    }
                    deck = newDeck.ToList();

                }
                else if (line.StartsWith("cut"))
                {
                    var cut = int.Parse(line.Substring(4));
                    if (cut < 0)
                        cut = deck.Count + cut;

                    var subdeck = deck.Take(cut).ToList();
                    deck = deck.Skip(cut).ToList();
                    deck.AddRange(subdeck);
                }

                if (sum != deck.Sum())
                    throw new Exception("hmmm");
            }
            
            if (deck.Count >= 2019)
                Console.WriteLine(deck.IndexOf(2019)); //5015 too low
            else
                Console.WriteLine(string.Join(" ", deck));
        }
    }
}