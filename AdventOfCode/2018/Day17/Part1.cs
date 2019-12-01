using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Drawing;
using System.Text.RegularExpressions;

namespace AdventOfCode2018.Day17
{
    public class Part1
    {
        private static Regex numbersRegex = new Regex(@"\d+");
        public static void Run()
        {
            var input = File.ReadAllLines("2018/Day17/Input.txt");

            var yMax = 0;
            var xMax = 0;
            var yMin = int.MaxValue;
            var groundSlice = new List<List<char>>();

            foreach (var line in input)
            {
                var numbers = numbersRegex.Matches(line);
                if (line.StartsWith("y"))
                {
                    var y = int.Parse(numbers[0].Value);
                    if (y > yMax)
                        yMax = y;
                    if (y < yMin)
                        yMin = y;

                    var x = int.Parse(numbers[2].Value);
                    if (x > xMax)
                        xMax = x;
                }
                else
                {
                    var x = int.Parse(numbers[0].Value);
                    if (x > xMax)
                        xMax = x;

                    var y1 = int.Parse(numbers[1].Value);
                    var y2 = int.Parse(numbers[2].Value);
                    if (y1 < yMin)
                        yMin = y1;
                    if (y2 > yMax)
                        yMax = y2;
                }
            }

            for (int y = 0; y <= yMax; y++)
            {
                groundSlice.Add(Enumerable.Repeat('.', xMax + 2).ToList());
            }

            foreach (var line in input)
            {
                var numbers = numbersRegex.Matches(line);
                var xlow = 0;
                var xhigh = 0;
                var ylow = 0;
                var yhigh = 0;
                if (line.StartsWith("y"))
                {
                    ylow = int.Parse(numbers[0].Value);
                    yhigh = ylow;
                    xlow = int.Parse(numbers[1].Value);
                    xhigh = int.Parse(numbers[2].Value);
                }
                else
                {
                    xlow = int.Parse(numbers[0].Value);
                    xhigh = xlow;
                    ylow = int.Parse(numbers[1].Value);
                    yhigh = int.Parse(numbers[2].Value);
                }

                for (int y = ylow; y <= yhigh; y++)
                {
                    for (int x = xlow; x <= xhigh; x++)
                    {
                        groundSlice[y][x] = '#';
                    }
                }
            }
            groundSlice[0][500] = '+';

            var finished = false;
            while (!finished)
            {
                var dropMoving = true;
                var dropX = 500;
                var dropY = 1;

                while (dropMoving)
                {
                    if (groundSlice[1][500] != '.') //DONE!
                    {
                        dropMoving = false;
                        finished = true;
                    }
                    else
                    {
                        while (dropY < yMax && groundSlice[dropY + 1][dropX] == '.')   //GO DOWN until stop
                            dropY++;

                        if (dropY == yMax || groundSlice[dropY + 1][dropX] == '|')
                        {
                            groundSlice[dropY][dropX] = '|';
                            dropMoving = false;
                        }
                        else if (dropX > 0 && groundSlice[dropY][dropX - 1] == '.')
                        {
                            while (dropX > 0 && groundSlice[dropY][dropX - 1] == '.') //GO LEFT until stop or line below
                            {
                                dropX--;
                                if (groundSlice[dropY + 1][dropX] == '.' || groundSlice[dropY + 1][dropX] == '|')   //Ahhh falling down
                                    break;
                            }
                            if (groundSlice[dropY + 1][dropX] != '.') //We didn't fall
                            {
                                groundSlice[dropY][dropX] = '|';
                                dropMoving = false;
                            }
                        }
                        else if (dropX < xMax && groundSlice[dropY][dropX + 1] == '.')
                        {
                            while (dropX < xMax && groundSlice[dropY][dropX + 1] == '.') //GO RIGHT until stop
                            {
                                dropX++;
                                if (groundSlice[dropY + 1][dropX] == '.' || groundSlice[dropY + 1][dropX] == '|')   //Ahhh falling down
                                    break;
                            }

                            if (groundSlice[dropY + 1][dropX] != '.') //We didn't fall
                            {
                                groundSlice[dropY][dropX] = '|';
                                dropMoving = false;
                            }
                        }
                        else //Can't down, left or right, freeze line (in bucket)
                        {
                            groundSlice[dropY][dropX] = '|';

                            while (groundSlice[dropY][--dropX] == '|')
                            {
                            }
                            if (groundSlice[dropY][dropX] != '#') //no edge on left side, Overflow!
                                break;
                            var minx = dropX + 1;
                            while (groundSlice[dropY][++dropX] == '|')
                            {
                            }
                            if (groundSlice[dropY][dropX] != '#') //no edge on right side, Overflow!
                                break;

                            while (dropX > minx)
                            {
                                groundSlice[dropY][--dropX] = '~';
                            }
                            dropMoving = false;
                        }
                    }
                }
            }

            var water = 0;
            foreach (var layer in groundSlice.Skip(yMin))
            {
                foreach (var c in layer)
                {
                    if (c == '|' || c == '~')
                        water++;
                }
            }
            
            Console.WriteLine(water);
            //DebugPrint(groundSlice, "D:/Temp/AdventOfCode2.txt");
        }

        private static void DebugPrint(List<List<char>> ground, string fileName)
        {
            File.WriteAllLines(fileName, ground.Select(line => string.Join("", line)));
        }
    }
}
