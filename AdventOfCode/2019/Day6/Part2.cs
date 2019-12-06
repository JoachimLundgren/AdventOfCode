using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace AdventOfCode2019.Day6
{
    public class Part2
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
                objects[orbit[1]].Orbit = objects[orbit[0]];
            }
            var sum = CalcShortestPath(null, objects["YOU"], objects["SAN"]);
            Console.WriteLine(sum - 2);
        }


        public static int CalcShortestPath(Obj prev, Obj origin, Obj target)
        {
            var potentialSteps = origin.Orbiting.ToList();
            if (origin.Orbit != null)
                potentialSteps.Add(origin.Orbit);
            if (prev != null)
                potentialSteps.Remove(prev);

            if (!potentialSteps.Any())
            {
                return 0;
            }
            else if (potentialSteps.Contains(target))
            {
                return 1;
            }
            else
            {
                var min = int.MaxValue;
                foreach (var step in potentialSteps)
                {
                    var steps = CalcShortestPath(origin, step, target);
                    if (steps > 0 && steps < min)
                        min = steps;
                }

                if (min == int.MaxValue)
                    return 0;
                else
                    return min + 1;
            }
        }
    }
}
