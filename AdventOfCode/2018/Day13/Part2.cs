using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace AdventOfCode2018.Day13
{
    public class Part2
    {
        public static void Run()
        {
            var input = File.ReadAllLines("2018/Day13/input.txt").Select(s => s.ToArray()).ToArray();
            var carts = new List<Cart>();
            for (int y = 0; y < input.Length; y++)
            {
                for (int x = 0; x < input[0].Length; x++)
                {
                    if (input[y][x] == '^')
                    {
                        carts.Add(new Cart(x, y, 1));
                        input[y][x] = '|';
                    }
                    else if (input[y][x] == '>')
                    {
                        carts.Add(new Cart(x, y, 2));
                        input[y][x] = '-';
                    }
                    else if (input[y][x] == 'v')
                    {
                        carts.Add(new Cart(x, y, 3));
                        input[y][x] = '|';
                    }
                    else if (input[y][x] == '<')
                    {
                        carts.Add(new Cart(x, y, 4));
                        input[y][x] = '-';
                    }
                }
            }


            while (true)
            {
                var cartsCopy = carts.OrderBy(c => c.X).ThenBy(c => c.Y).ToList();
                foreach (var cart in cartsCopy)
                {
                    var track = input[cart.Y][cart.X];
                    if (track == '|' || track == '-')
                    {
                        cart.Move();
                    }
                    else if (track == '/' || track == '\\')
                    {
                        cart.Turn(track);
                        cart.Move();
                    }
                    else if (track == '+')
                    {
                        cart.Rotate();
                        cart.Move();
                    }
                    else
                    {
                        throw new Exception();
                    }

                    var collidingCarts = cartsCopy.Where(c => c.X == cart.X && c.Y == cart.Y);
                    if (collidingCarts.Count() > 1)
                    {
                        Console.WriteLine($"CRASH AT {cart.X},{cart.Y}");
                        carts.Remove(collidingCarts.ElementAt(0));
                        carts.Remove(collidingCarts.ElementAt(1));
                        Console.WriteLine($"Remaining Carts {carts.Count}");
                    }

                    if (carts.Count == 1)
                    {
                        Console.WriteLine($"Last cart @ {carts.Single().X},{carts.Single().Y}");
                        return;
                    }
                    if (!carts.Any())
                    {
                        throw new Exception();
                    }
                }
            }
        }

        private class Cart
        {
            public int X { get; private set; }
            public int Y { get; private set; }
            public int Direction { get; set; }
            public int Rotations { get; set; }

            public Cart(int x, int y, int direction)
            {
                X = x;
                Y = y;
                Direction = direction;
                Rotations = 0;
            }

            public void Move()
            {
                if (Direction == 1) Y--;
                else if (Direction == 2) X++;
                else if (Direction == 3) Y++;
                else if (Direction == 4) X--;
            }

            public void Rotate()
            {
                if (Rotations == 0)
                {
                    if (Direction == 1)
                        Direction = 4;
                    else
                        Direction -= 1;
                }
                else if (Rotations == 2)
                {
                    if (Direction == 4)
                        Direction = 1;
                    else
                        Direction += 1;
                }

                Rotations++;
                if (Rotations == 3)
                    Rotations = 0;
            }

            public void Turn(char track)
            {
                if (Direction == 1)
                {
                    if (track == '/')
                        Direction = 2;
                    else if (track == '\\')
                        Direction = 4;
                    else
                        throw new Exception();
                }
                else if (Direction == 2)
                {
                    if (track == '/')
                        Direction = 1;
                    else if (track == '\\')
                        Direction = 3;
                    else
                        throw new Exception();
                }
                else if (Direction == 3)
                {
                    if (track == '/')
                        Direction = 4;
                    else if (track == '\\')
                        Direction = 2;
                    else
                        throw new Exception();
                }
                else if (Direction == 4)
                {
                    if (track == '/')
                        Direction = 3;
                    else if (track == '\\')
                        Direction = 1;
                    else
                        throw new Exception();
                }
            }
        }
    }
}
