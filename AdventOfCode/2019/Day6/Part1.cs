using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;

namespace AdventOfCode2019.Day6
{
    public class Obj
    {
        public string Name { get; }
        public Obj Orbit { get; set; }   //Parent
        public List<Obj> Orbiting { get; } //Child
        public int Orbitcount { get; set; }
        public Obj(string name)
        {
            Name = name;
            Orbiting = new List<Obj>();
        }
    }
    public class Part1
    {
        public static void Run()
        {
            var input = File.ReadAllLines("2019/Day6/Input.txt");
            var orbits = input.Select(line => line.Split(')')).ToList();

            var objects = new Dictionary<string, Obj>();
            foreach (var orbit in orbits)
            {
                if (!objects.ContainsKey(orbit[0]))
                    objects.Add(orbit[0], new Obj(orbit[0]));
                if (!objects.ContainsKey(orbit[1]))
                    objects.Add(orbit[1], new Obj(orbit[1]));

                objects[orbit[0]].Orbiting.Add(objects[orbit[1]]);
            }
            var sum = SetOrbits(objects["COM"]);
            Console.WriteLine(sum);
        }

        public static int SetOrbits(Obj obj)
        {
            var sum = 0;
            foreach (var orbits in obj.Orbiting)
            {
                orbits.Orbitcount = obj.Orbitcount + 1;
                sum += orbits.Orbitcount;
                sum += SetOrbits(orbits);
            }
            return sum;
        }
    }
}
