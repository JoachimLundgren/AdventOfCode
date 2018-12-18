using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using System.Drawing;

namespace AdventOfCode.Day4
{
    public class Part2
    {
        private static Regex dateRegex = new Regex(@"(\d\d\d\d)-(\d\d)-(\d\d) (\d\d):(\d\d)");
        private static Regex guardNumberRegex = new Regex(@"(.*)#(\d*)(.*)");
        public static void Run()
        {
            var input = File.ReadAllLines("Day4/Input.txt");

            var entries = input.Select(i => new GuardEntry(i)).OrderBy(g => g.Timestamp).ToList();

            var guards = new List<Guard>();
            Guard guard = null;
            int sleep = -1;
            for (int i = 0; i < entries.Count; i++)
            {
                var entry = entries[i];
                if (entry.Description.StartsWith('G'))
                {
                    var matches = guardNumberRegex.Matches(entry.Description);
                    var guardId = int.Parse(matches.First().Groups[2].Value);
                    guard = guards.FirstOrDefault(g => g.Id == guardId);
                    if (guard == null)
                    {
                        guard = new Guard(guardId);
                        guards.Add(guard);
                    }
                }
                else if (entry.Description.StartsWith('f'))
                {
                    if (sleep != -1)
                        throw new Exception();

                    sleep = entry.Timestamp.Minute;
                }
                else if (entry.Description.StartsWith('w'))
                {
                    if (sleep == -1)
                        throw new Exception();

                    guard.AddSleep(sleep, entry.Timestamp.Minute);
                    sleep = -1;
                }
                else
                {
                    throw new Exception();
                }
            }

            var sleepyGuard = guards.OrderByDescending(g => g.GetMostSleepyMinute().Value).First();

            Console.WriteLine(sleepyGuard.Id * sleepyGuard.GetMostSleepyMinute().Key);
        }




        private class Guard
        {
            public int Id { get; }
            public List<Tuple<int, int>> Sleeptimes { get; }

            public Guard(int id)
            {
                Id = id;
                Sleeptimes = new List<Tuple<int, int>>();
            }

            public void AddSleep(int sleep, int wakeup)
            {
                Sleeptimes.Add(new Tuple<int, int>(sleep, wakeup));
            }

            public int GetTotalSleep()
            {
                return Sleeptimes.Sum(s => s.Item2 - s.Item1);
            }

            public KeyValuePair<int, int> GetMostSleepyMinute()
            {
                var minuteDict = Enumerable.Range(0, 59).ToDictionary(i => i, i => 0);

                foreach (var sleep in Sleeptimes)
                {
                    for (int i = sleep.Item1; i < sleep.Item2; i++)
                    {
                        minuteDict[i]++;
                    }
                }

                return minuteDict.OrderByDescending(m => m.Value).First();
            }
        }

        private class GuardEntry
        {
            public DateTime Timestamp { get; }
            public string Description { get; }

            public GuardEntry(string input)
            {
                var matches = dateRegex.Matches(input);
                Timestamp = DateTime.Parse(matches.Single().Value);
                Description = input.Split(']')[1].Trim();
            }
        }
    }
}
