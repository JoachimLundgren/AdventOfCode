using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using System.Drawing;

namespace AdventOfCode.Day9
{
    public class Part2
    {
        public static void Run()
        {
            // var input = File.ReadAllText("Day8/Input.txt");
            var numPlayers = 418;
            var points = 71339 * 100;
            var players = new long[numPlayers];



            var marbles = new LinkedList<int>();
            var currentMarble = marbles.AddFirst(0);
            for (int i = 1; i <= points; i++)
            {
                var multipleOf23 = i % 23 == 0;
                if (multipleOf23)
                {
                    currentMarble = currentMarble.Previous ?? marbles.Last;
                    currentMarble = currentMarble.Previous ?? marbles.Last;
                    currentMarble = currentMarble.Previous ?? marbles.Last;
                    currentMarble = currentMarble.Previous ?? marbles.Last;
                    currentMarble = currentMarble.Previous ?? marbles.Last;
                    currentMarble = currentMarble.Previous ?? marbles.Last;
                    currentMarble = currentMarble.Previous ?? marbles.Last;

                    var player = i % numPlayers;
                    players[player] += i + currentMarble.Value;

                    var old = currentMarble;
                    currentMarble = currentMarble.Next;
                    marbles.Remove(old);
                }
                else
                {
                    currentMarble = marbles.AddAfter(currentMarble.Next ?? marbles.First, i);
                }
            }


            Console.WriteLine(players.Max()); //115151 to low
        }
    }
}
