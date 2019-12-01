using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;

namespace AdventOfCode2018.Day9
{
    public class Part1
    {
        public static void Run()
        {
            // var input = File.ReadAllText("2018/Day8/Input.txt");
            var numPlayers = 418;
            var points = 71339;
            var players = new int[numPlayers];



            var currentMarble = 0;

            var marbles = new List<int>() { 0 };
            for (int i = 1; i <= points; i++)
            {
                var multipleOf23 = i % 23 == 0;
                if (multipleOf23)
                {
                    var player = i % numPlayers;
                    players[player] += i;
                    currentMarble -= 7;
                    if (currentMarble < 0)
                        currentMarble += marbles.Count;

                    players[player] += marbles[currentMarble];
                    marbles.RemoveAt(currentMarble);
                }
                else
                {
                    currentMarble += 2;
                    currentMarble %= marbles.Count;
                    if (currentMarble == 0)
                    {
                        marbles.Add(i);
                        currentMarble = marbles.Count - 1;
                    }
                    else
                    {
                        marbles.Insert(currentMarble, i);
                    }
                }
            }


            Console.WriteLine(players.Max()); //115151 to low
        }
    }
}
